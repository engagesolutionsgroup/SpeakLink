using Foundation;
using LinkedIn.Hakawai;
using UIKit;

namespace SpeakLink.Controls.MaciOS.SpanStyle;

public abstract class TraitFormattedToolbarSpanStyle : FormattedToolbarSpanStyle
{
    private static readonly NSString OriginalFontKey = new NSString("NSOriginalFont");
    public abstract UIFontDescriptorSymbolicTraits SymbolicTrait { get; }

    protected TraitFormattedToolbarSpanStyle(SpeakLinkRichTextView richTextView)
        : base(richTextView)
    {
        RichTextView = richTextView;
    }

    public override void ApplyStyle(NSRange range, HKWTextView textView, bool newState)
    {
        RichTextView.TransformTextAtRange(range, s =>
        {
            var newString = new NSMutableAttributedString(s);

            void UpdateFont(NSObject value, NSRange effectiveRange, ref bool stop)
            {
                UIFont font = value as UIFont ?? RichTextView.Font ?? RichTextView.GetFontSetByApp();
                var traits = font.FontDescriptor.SymbolicTraits;

                // Determine new traits based on whether newState is bold or not
                traits = newState ? (traits | SymbolicTrait) : (traits & ~SymbolicTrait);

                // Get the original font if available, for handling emoji
                var originalFont = s.GetAttribute(OriginalFontKey, effectiveRange.Location, out _) as UIFont;
                UIFont newFont;

                if (originalFont != null && originalFont.FamilyName != font?.FamilyName)
                {
                    // If originalFont exists we should update the OriginalFont attribute in case of emoji, 
                    // for future: check for equals originalFont.FamilyName to unsupported character font (Apple Emoji)
                    var originalTraits = originalFont.FontDescriptor.SymbolicTraits;
                    originalTraits = newState ? (originalTraits | SymbolicTrait) : (originalTraits & ~SymbolicTrait);
                    var newOriginalFontDescriptor = originalFont.FontDescriptor.CreateWithTraits(originalTraits);
                    newFont = UIFont.FromDescriptor(newOriginalFontDescriptor, originalFont.PointSize);
                    newString.AddAttribute(OriginalFontKey, newFont, effectiveRange);
                }
                else
                {
                    // If it's text, just update the font attribute
                    var newFontDescriptor = font.FontDescriptor.CreateWithTraits(traits);
                    newFont = UIFont.FromDescriptor(newFontDescriptor, font.PointSize);
                    newString.AddAttribute(UIStringAttributeKey.Font, newFont, effectiveRange);
                }
            }

            s.EnumerateAttribute(UIStringAttributeKey.Font, new NSRange(0, s.Length), NSAttributedStringEnumeration.None, UpdateFont);

            return newString;
        });
        textView.SelectedRange = range;
    }

    public override void UpdateTypingAttributes(NSDictionary? typingAttributes)
    {
         UpdateSymbolicTraitForTypingAttribute(RichTextView.CustomTypingAttributes, SymbolicTrait);
         RichTextView.CustomTypingAttributes = RichTextView.CustomTypingAttributes;
    }

    public void UpdateSymbolicTraitForTypingAttribute(NSMutableDictionary typingAttributes,
        UIFontDescriptorSymbolicTraits traitsToUpdate)
    {
        var inputFont = (typingAttributes[UIStringAttributeKey.Font] as UIFont) ?? RichTextView.Font ?? RichTextView.GetFontSetByApp();
        if (!typingAttributes.TryGetValue(UIStringAttributeKey.Font, out var existingFontNsObject) ||
            existingFontNsObject is not UIFont existingFont)
        {
            var newFont = UIFont.FromDescriptor(inputFont.FontDescriptor, inputFont.PointSize);
            typingAttributes.SetValueForKey(newFont, UIStringAttributeKey.Font);
            existingFont = newFont;
        }

        var existingTraits = existingFont.FontDescriptor.SymbolicTraits;
        var hasTrait = (existingTraits & traitsToUpdate) == traitsToUpdate;

        if (Checked == hasTrait)
            return;

        var newTraits = Checked ? existingTraits | traitsToUpdate : existingTraits & ~traitsToUpdate;
        var newFontDescriptor = inputFont.FontDescriptor.CreateWithTraits(newTraits);
        var updatedFont = UIFont.FromDescriptor(newFontDescriptor, inputFont.PointSize);

        typingAttributes.SetValueForKey(updatedFont, UIStringAttributeKey.Font);
    }


    public override bool CheckSpanForRange(NSRange? selectedRange)
    {
        if (selectedRange == null)
            return false;

        bool hasTrait = true;

        // Loop through the characters in the selected range
        RichTextView.AttributedText.EnumerateAttributes(
            selectedRange.Value,
            NSAttributedStringEnumeration.None,
            (NSDictionary attrs, NSRange range, ref bool stop) =>
            {
                // Check for bold attribute
                var originalFont = attrs[OriginalFontKey] as UIFont;
                var font = originalFont ?? attrs[UIStringAttributeKey.Font] as UIFont;

                if (font != null)
                {
                    if ((font.FontDescriptor.SymbolicTraits & SymbolicTrait) != 0)
                        return;

                    hasTrait = false;
                    stop = true;
                }
                else
                {
                    hasTrait = false;
                    stop = true;
                }
            });

        return hasTrait;
    }
}