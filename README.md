# SpeakLink Mention Editor Library for .NET MAUI
[![NuGet version (SpeakLink)](https://img.shields.io/nuget/v/SpeakLink.svg?style=flat-square)](https://www.nuget.org/packages/SpeakLink/)
## Overview
The SpeakLink Mention Editor is an advanced .NET MAUI library that enhances text editor functionality with support for @mentions. Designed to bring the intuitive and flexible mention capabilities found on social media platforms to .NET MAUI apps, it offers a seamless integration for developers. It is up to you how you want the mention/hashtag picker to look; see the example project to see how the most common approach, 'mention list above input' is implemented.

![GIF image that displays sample project mention picking flow iOS](/gif/ios.gif)
![GIF image that displays sample project mention picking flow Android](/gif/android.gif)

## Usage
1. **Add the Library**: Integrate the `SpeakLink.Mention` library into your project either through NuGet or by adding a direct reference.
2. **Namespace Declaration**: In your XAML, declare the SpeakLink Mention Editor's namespace:
   ```xml
   xmlns:mention="clr-namespace:SpeakLink.Mention;assembly=SpeakLink"
   ```
3. **Add the Editor**: Implement the `MentionEditor` control in your XAML:
   ```xml
   <mention:MentionEditor x:Name="editor" />
   ```
4. **Configure Key Attributes**: Customize the `MentionEditor` in your XAML to suit your application's requirements. Essential attributes include:
   - `MentionSearchCommand`: A ViewModel command that initiates the mention search.
   - `IsSuggestionsPopupVisible`: A Boolean that dynamically adjusts to control the visibility of mention suggestions.
   - `ExplicitCharacters`: Characters, like "@", used to trigger mention suggestions.

   The `MentionSearchCommand` is triggered with `MentionSearchEventArgs`, which contains `ControlCharacter` (like `ExplicitCharacter`, e.g., "@") and `MentionQuery` (e.g., 'Dav'). Your task is to filter suggestions based on the `MentionQuery` and display them, for example, using a `CollectionView` or another selector (refer to the Sample project for more details).

5. **Handling User Selections**: When a user selects an item from the CollectionView/ListView/BindableLayout you used to display list of mentions, invoke:
   ```xml
   editor.InsertMention(id, mentionText);
   ```
6. **FormattedText Property Updates**: The `SpeakLink.Mention.MentionEditor`'s `FormattedText` property updates dynamically as the user types, deletes, or inserts a mention. Each mention is represented as a distinct `MentionSpan` that contains `Id` and `Text` as `mentionText`.
7. **Keyboard closing**: The keyboard will close as soon as focus is lost. Unfortunately, I wasn't able to integrate with [HideSoftInputOnTapped property of ContentPage](https://learn.microsoft.com/en-us/dotnet/api/microsoft.maui.controls.contentpage.hidesoftinputontapped?view=net-maui-8.0#microsoft-maui-controls-contentpage-hidesoftinputontapped) because HideSoftInputOnTapped is internal class for MAUI, moreover it doesnt support adding additional ignore area for picker, so you have to add GestureRecognizer on your root element and add something like:
      ```csharp
    private void HideKeyboard(object? sender, TappedEventArgs e)
    {
        if (!MentionEditor.IsFocused)
            return;
        
        if (MentionEditor.IsSuggestionsPopupVisible
            && e.GetPosition(MentionPickerView) is { } position
            && position.InsideElement(MentionPickerView))
        {
            return;
        }

        MentionEditor.Unfocus();
    }
   ```

## Image Insert
Use ImageInsertCommand that passes string as filePath that user wants to insert
![GIF image that insert support on Android](/gif/android_insert.gif)