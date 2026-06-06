using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FNAI
{
    /// <summary>
    /// Logique d'interaction pour PopupInvasion.xaml
    /// </summary>
    public partial class PopupInvasion : UserControl
    {
        public event EventHandler DemandeDuplication;
        public event EventHandler DemandeFermeture;

        private int clicsRequis = 1;

        public PopupInvasion(int pointsDeVie)
        {
            InitializeComponent();
            clicsRequis = pointsDeVie;
            MettreAJourAffichage();
        }

        private void BoutonCroix_Click(object sender, RoutedEventArgs e)
        {
            if (clicsRequis > 1)
            {
                clicsRequis--;
                MettreAJourAffichage();

                DemandeDuplication?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                DemandeFermeture?.Invoke(this, EventArgs.Empty);
            }
        }

        private void MettreAJourAffichage()
        {
            TxtVies.Text = $"Clics requis : {clicsRequis}";
        }
    }
}
