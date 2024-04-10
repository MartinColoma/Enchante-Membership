using System.Windows.Forms;


namespace EnchanteMembership
{
    internal class Billing
    {
        private Panel Sub;
        private Panel History;
        private Panel Method;



        public Billing(Panel sub, Panel history, Panel method)
        {
            Sub = sub;
            History = history;
            Method = method;


        }

        public void PanelShow(Panel panelToShow)
        {
            Sub.Hide();
            History.Hide();
            Method.Hide();

            panelToShow.Show();
        }
    }
}
