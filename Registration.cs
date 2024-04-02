using System.Windows.Forms;


namespace EnchanteMembership
{
    internal class Registration
    {
        private Panel Regis;
        private Panel Regular;
        private Panel Premium;
        private Panel SuperVIP;


        public Registration(Panel regis, Panel reg, Panel prem, Panel svip)
        {
            Regis = regis;
            Regular = reg;
            Premium = prem;
            SuperVIP = svip;

        }

        public void PanelShow(Panel panelToShow)
        {
            Regis.Hide();
            Regular.Hide();
            Premium.Hide();
            SuperVIP.Hide();

            panelToShow.Show();
        }
    }
}
