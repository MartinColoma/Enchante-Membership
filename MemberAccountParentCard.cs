using System.Windows.Forms;


namespace Enchante_Membership
{
    internal class MemberAccountParentCard
    {
        private Panel Home;
        private Panel Appt;
        private Panel Bill;
        private Panel Reviews;
        private Panel Profile;


        public MemberAccountParentCard(Panel bahay, Panel appt, Panel gastos, Panel reklamo, Panel profile)
        {
            Home = bahay;
            Appt = appt;
            Bill = gastos;
            Reviews = reklamo;
            Profile = profile;

        }

        public void PanelShow(Panel panelToShow)
        {
            Home.Hide();
            Appt.Hide();
            Bill.Hide();
            Reviews.Hide();
            Profile.Hide();

            panelToShow.Show();
        }
    }
}
