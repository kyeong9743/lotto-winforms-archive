using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lotto
{
    public partial class choice : Form
    {
        public choice()
        {
            InitializeComponent();
            SharedUtils.InitializeDatabase();
        }

        private void lotto_input_Click(object sender, EventArgs e)
        {
            lotto_input_form lotto_input_form = new lotto_input_form();
            lotto_input_form.ShowDialog();
        }

        private void adminloginbtn_Click(object sender, EventArgs e)
        {
            if (ID.Text == "1") // 하드코딩
            {
                admin_raffle admin_raffle = new admin_raffle();
                admin_raffle.ShowDialog();
            }
            else
            {
                MessageBox.Show("접근거부", "컴퓨터공학과");
            }
            
        }

        private void choice_Load(object sender, EventArgs e)
        {

        }
    }
}
