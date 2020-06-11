using RadarFamilyCore.ViewModels.Dto;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RadarFamilyCore.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomeTabbedPage : TabbedPage
    {
        public HomeTabbedPage(DtoResultLogin usuarioLogado)
        {
            InitializeComponent();
        }
    }
}