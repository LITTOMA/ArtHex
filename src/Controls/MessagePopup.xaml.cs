namespace ArtHex.Controls;

public partial class MessagePopup : CommunityToolkit.Maui.Views.Popup
{
    public MessagePopup(string message)
    {
        InitializeComponent();

        this.message.Text = message;
    }

    private void OkButton_Clicked(object sender, EventArgs e)
    {
        Close(true);
    }
}