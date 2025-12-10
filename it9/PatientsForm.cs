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
    public partial class PatientsForm : Form
    {
        private List<PatientInfo> patients = new List<PatientInfo>();

        public PatientsForm()
        {
            InitializeComponent();
            LoadPatients();
        }

        private void LoadPatients()
        {
            patients.Clear();

            // Молодые пациенты
            patients.Add(new PatientInfo(1, "Иванов", "Иван", "Иванович", new DateTime(1990, 5, 15), "Студент", "Удовлетворительное"));
            patients.Add(new PatientInfo(2, "Петрова", "Анна", "Сергеевна", new DateTime(1995, 8, 22), "Рабочий", "Стабильное"));
            patients.Add(new PatientInfo(3, "Сидоров", "Алексей", "Петрович", new DateTime(1988, 3, 10), "Инженер", "Хорошее"));

            // Взрослые работающие
            patients.Add(new PatientInfo(4, "Козлов", "Дмитрий", "Викторович", new DateTime(1980, 7, 18), "Менеджер", "Удовлетворительное"));
            patients.Add(new PatientInfo(5, "Николаева", "Ольга", "Игоревна", new DateTime(1975, 11, 5), "Бухгалтер", "Стабильное"));
            patients.Add(new PatientInfo(6, "Федоров", "Сергей", "Анатольевич", new DateTime(1978, 2, 28), "Водитель", "Тяжелое"));

            // Пожилые пациенты
            patients.Add(new PatientInfo(7, "Васнецова", "Елена", "Владимировна", new DateTime(1955, 12, 10), "Пенсионер", "Тяжелое"));
            patients.Add(new PatientInfo(8, "Григорьев", "Андрей", "Борисович", new DateTime(1960, 4, 3), "Пенсионер", "Удовлетворительное"));
            patients.Add(new PatientInfo(9, "Морозова", "Татьяна", "Леонидовна", new DateTime(1965, 9, 17), "Пенсионер", "Стабильное"));

            // Дети и подростки
            patients.Add(new PatientInfo(10, "Белов", "Павел", "Сергеевич", new DateTime(2010, 6, 25), "Школьник", "Хорошее"));
            patients.Add(new PatientInfo(11, "Кузнецова", "Ирина", "Александровна", new DateTime(2008, 1, 14), "Учащаяся", "Удовлетворительное"));
            patients.Add(new PatientInfo(12, "Семенов", "Виктор", "Геннадьевич", new DateTime(2012, 3, 30), "Дошкольник", "Стабильное"));

            // Безработные и другие статусы
            patients.Add(new PatientInfo(13, "Павлова", "Наталья", "Михайловна", new DateTime(1985, 10, 8), "Безработный", "Тяжелое"));
            patients.Add(new PatientInfo(14, "Киселев", "Артем", "Олегович", new DateTime(1992, 7, 22), "Студент", "Хорошее"));
            patients.Add(new PatientInfo(15, "Орлова", "Светлана", "Дмитриевна", new DateTime(1972, 5, 19), "Декретный отпуск", "Удовлетворительное"));

            // Разные состояния
            patients.Add(new PatientInfo(16, "Жуков", "Максим", "Ильич", new DateTime(1968, 8, 11), "Инвалид", "Критическое"));
            patients.Add(new PatientInfo(17, "Романова", "Екатерина", "Алексеевна", new DateTime(1998, 12, 3), "Студент", "Хорошее"));
            patients.Add(new PatientInfo(18, "Дмитриев", "Константин", "Викторович", new DateTime(1977, 6, 7), "Рабочий", "Стабильное"));

            // Разнообразные фамилии
            patients.Add(new PatientInfo(19, "Андреев", "Михаил", "Олегович", new DateTime(1983, 4, 12), "Программист", "Хорошее"));
            patients.Add(new PatientInfo(20, "Сергеева", "Марина", "Владимировна", new DateTime(1991, 9, 28), "Врач", "Удовлетворительное"));

            RefreshDataGrid();
        }

        private void RefreshDataGrid()
        {
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = patients;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Функция добавления пациента", "Информация",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var result = MessageBox.Show("Удалить выбранных пациентов?", "Подтверждение",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    var selectedRows = dataGridView1.SelectedRows.Cast<DataGridViewRow>().ToList();
                    var idsToRemove = selectedRows.Select(row => ((PatientInfo)row.DataBoundItem).ID).ToList();

                    patients.RemoveAll(p => idsToRemove.Contains(p.ID));
                    RefreshDataGrid();

                    MessageBox.Show($"Удалено {selectedRows.Count} пациентов", "Информация",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Выберите пациентов для удаления", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private class PatientInfo
        {
            public int ID { get; set; }
            public string Фамилия { get; set; }
            public string Имя { get; set; }
            public string Отчество { get; set; }
            public DateTime Дата_рождения { get; set; }
            public string Социальный_статус { get; set; }
            public string Состояние { get; set; }

            public PatientInfo(int id, string lastName, string firstName, string middleName,
                             DateTime birthDate, string socialStatus, string condition)
            {
                ID = id;
                Фамилия = lastName;
                Имя = firstName;
                Отчество = middleName;
                Дата_рождения = birthDate;
                Социальный_статус = socialStatus;
                Состояние = condition;
            }
        }
    }
}
