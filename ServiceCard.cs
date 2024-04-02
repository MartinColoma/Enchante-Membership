using System.Windows.Forms;


namespace EnchanteMembership
{
    internal class ServiceCard
    {
        private Panel ServiceType;
        private Panel HairStyle;
        private Panel FaceSkin;
        private Panel NailCare;
        private Panel Spa;
        private Panel Massage;


        public ServiceCard(Panel Type, Panel Hair, Panel Face, Panel Nail, Panel spa, Panel massage)
        {
            ServiceType = Type;
            HairStyle = Hair;
            FaceSkin = Face;
            NailCare = Nail;
            Spa = spa;
            Massage = massage;

        }

        public void PanelShow(Panel panelToShow)
        {
            ServiceType.Hide();
            HairStyle.Hide();
            FaceSkin.Hide();
            NailCare.Hide();
            Spa.Hide();
            Massage.Hide();

            panelToShow.Show();
        }
    }
}
