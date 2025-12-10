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
    public partial class AddDoctorForm : Form
    {
        public AddDoctorForm()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLastName.Text) ||
                string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                MessageBox.Show("Фамилия и имя обязательны для заполнения", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void AddDoctor_Load(object sender, EventArgs e)
        {
            // Инициализация
        }

        // Свойства для доступа к данным формы
        public string LastName => txtLastName.Text;
        public string FirstName => txtFirstName.Text;
        public string MiddleName => txtMiddleName.Text;
        public string Position => txtPosition.Text;
        public string Qualification => txtQualification.Text;
        public string Specialization => txtSpecialization.Text;
    }
}
