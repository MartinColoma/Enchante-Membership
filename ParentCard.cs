﻿using System.Windows.Forms;

namespace EnchanteMembership
{
    internal class ParentCard
    {
        private Panel HomePage;
        private Panel Member;


        public ParentCard(Panel home, Panel member)
        {
            HomePage = home;
            Member = member;

        }

        public void PanelShow(Panel panelToShow)
        {
            HomePage.Hide();
            Member.Hide();

            panelToShow.Show();
        }
    }
}
