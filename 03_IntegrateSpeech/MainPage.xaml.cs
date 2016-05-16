using System;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Trivadis.IoT.Uwp.TalkToMe
{
  public sealed partial class MainPage : Page
  {
    private MediaElement _mediaElement;

    public MainPage()
    {
      this.InitializeComponent();
      _mediaElement = new MediaElement();
    }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
      var speech = new SpeechSynthesizer();

      var stream = await speech.SynthesizeTextToStreamAsync(txtInput.Text);
      _mediaElement.SetSource(stream, stream.ContentType);
      _mediaElement.Play();
    }
  }
}
