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
    public partial class DoctorsForm : Form
    {
        private List<DoctorInfo> doctors = new List<DoctorInfo>();

        public DoctorsForm()
        {
            InitializeComponent();
            LoadDoctors();
        }

        private void LoadDoctors()
        {
            // Разнообразные врачи разных специальностей
            doctors.Clear();

            // Терапевты
            doctors.Add(new DoctorInfo(1, "Иванов", "Иван", "Иванович", "Терапевт", "Высшая", "Терапия"));
            doctors.Add(new DoctorInfo(2, "Петрова", "Мария", "Сергеевна", "Врач-терапевт", "Первая", "Общая практика"));
            doctors.Add(new DoctorInfo(3, "Сидоров", "Алексей", "Петрович", "Терапевт участковый", "Высшая", "Терапия"));

            // Хирурги
            doctors.Add(new DoctorInfo(4, "Козлов", "Дмитрий", "Викторович", "Хирург", "Высшая", "Общая хирургия"));
            doctors.Add(new DoctorInfo(5, "Николаева", "Ольга", "Игоревна", "Хирург", "Первая", "Хирургия"));
            doctors.Add(new DoctorInfo(6, "Федоров", "Сергей", "Анатольевич", "Травматолог-хирург", "Высшая", "Травматология"));

            // Кардиологи
            doctors.Add(new DoctorInfo(7, "Васнецова", "Елена", "Владимировна", "Кардиолог", "Высшая", "Кардиология"));
            doctors.Add(new DoctorInfo(8, "Григорьев", "Андрей", "Борисович", "Врач-кардиолог", "Вторая", "Кардиология"));

            // Неврологи
            doctors.Add(new DoctorInfo(9, "Морозова", "Татьяна", "Леонидовна", "Невролог", "Высшая", "Неврология"));
            doctors.Add(new DoctorInfo(10, "Белов", "Павел", "Сергеевич", "Невропатолог", "Первая", "Неврология"));

            // Отоларингологи
            doctors.Add(new DoctorInfo(11, "Кузнецова", "Ирина", "Александровна", "ЛОР-врач", "Высшая", "Отоларингология"));
            doctors.Add(new DoctorInfo(12, "Семенов", "Виктор", "Геннадьевич", "Оториноларинголог", "Первая", "ЛОР"));

            // Офтальмологи
            doctors.Add(new DoctorInfo(13, "Павлова", "Наталья", "Михайловна", "Офтальмолог", "Высшая", "Офтальмология"));

            // Гастроэнтерологи
            doctors.Add(new DoctorInfo(14, "Киселев", "Артем", "Олегович", "Гастроэнтеролог", "Вторая", "Гастроэнтерология"));

            // Педиатры
            doctors.Add(new DoctorInfo(15, "Орлова", "Светлана", "Дмитриевна", "Педиатр", "Высшая", "Педиатрия"));
            doctors.Add(new DoctorInfo(16, "Жуков", "Максим", "Ильич", "Врач-педиатр", "Первая", "Детские болезни"));

            RefreshDataGrid();
        }

        private void RefreshDataGrid()
        {
            doctorsGrid.DataSource = null;
            doctorsGrid.DataSource = doctors;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var form = new AddDoctorForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                int newId = doctors.Count > 0 ? doctors.Max(d => d.ID) + 1 : 1;
                doctors.Add(new DoctorInfo(
                    newId,
                    form.LastName,
                    form.FirstName,
                    form.MiddleName,
                    form.Position,
                    form.Qualification,
                    form.Specialization
                ));

                RefreshDataGrid();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (doctorsGrid.SelectedRows.Count > 0)
            {
                var result = MessageBox.Show("Удалить выбранных врачей?", "Подтверждение",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    var selectedRows = doctorsGrid.SelectedRows.Cast<DataGridViewRow>().ToList();
                    var idsToRemove = selectedRows.Select(row => ((DoctorInfo)row.DataBoundItem).ID).ToList();

                    doctors.RemoveAll(d => idsToRemove.Contains(d.ID));
                    RefreshDataGrid();

                    MessageBox.Show($"Удалено {selectedRows.Count} врачей", "Информация",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Выберите врачей для удаления", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private class DoctorInfo
        {
            public int ID { get; set; }
            public string Фамилия { get; set; }
            public string Имя { get; set; }
            public string Отчество { get; set; }
            public string Должность { get; set; }
            public string Квалификация { get; set; }
            public string Специализация { get; set; }

            public DoctorInfo(int id, string lastName, string firstName, string middleName,
                            string position, string qualification, string specialization)
            {
                ID = id;
                Фамилия = lastName;
                Имя = firstName;
                Отчество = middleName;
                Должность = position;
                Квалификация = qualification;
                Специализация = specialization;
            }
        }
    }
}
