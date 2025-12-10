using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace it9
{
    public partial class MainMenu : Form  // Изменил с MainForm на MainMenu
    {
        public MainMenu()
        {
            InitializeComponent();
        }

        private void btnPatients_Click(object sender, EventArgs e)
        {
            var form = new PatientsForm();
            form.ShowDialog();
        }

        private void btnDoctors_Click(object sender, EventArgs e)
        {
            var form = new DoctorsForm();
            form.ShowDialog();
        }

        private void btnDiagnoses_Click(object sender, EventArgs e)
        {
            var form = new DiagnosesForm();
            form.ShowDialog();
        }

        private void btnAmbulatoryPatients_Click(object sender, EventArgs e)
        {
            var form = new AmbulatoryPatientsForm();
            form.ShowDialog();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void MainMenu_Load(object sender, EventArgs e)
        {
            // Инициализация при загрузке формы
        }
    }
}
