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
    public partial class DiagnosesForm : Form
    {
        private List<DiagnosisInfo> diagnoses = new List<DiagnosisInfo>();

        public DiagnosesForm()
        {
            InitializeComponent();
            LoadDiagnoses();
        }

        private void LoadDiagnoses()
        {
            diagnoses.Clear();

            // 1. Амбулаторные случаи (ОРВИ, простуда и т.д.)
            diagnoses.Add(new DiagnosisInfo(1, "Иванов Иван Иванович", "Петрова Мария Сергеевна",
                "ОРВИ", "Амбулаторное", new DateTime(2024, 12, 31), "Нет", new DateTime(2024, 12, 1)));

            diagnoses.Add(new DiagnosisInfo(2, "Петрова Анна Сергеевна", "Иванов Иван Иванович",
                "Грипп", "Амбулаторное", new DateTime(2025, 1, 10), "Да", new DateTime(2024, 12, 5)));

            diagnoses.Add(new DiagnosisInfo(3, "Сидоров Алексей Петрович", "Морозова Татьяна Леонидовна",
                "Гипертоническая болезнь", "Амбулаторное", new DateTime(2025, 6, 30), "Да", new DateTime(2024, 1, 15)));

            diagnoses.Add(new DiagnosisInfo(4, "Козлов Дмитрий Викторович", "Васнецова Елена Владимировна",
                "Стенокардия", "Амбулаторное", new DateTime(2025, 12, 31), "Да", new DateTime(2024, 3, 20)));

            diagnoses.Add(new DiagnosisInfo(5, "Николаева Ольга Игоревна", "Киселев Артем Олегович",
                "Гастрит", "Амбулаторное", new DateTime(2025, 3, 15), "Нет", new DateTime(2024, 11, 10)));

            // 2. Стационарные случаи (операции, серьезные заболевания)
            diagnoses.Add(new DiagnosisInfo(6, "Федоров Сергей Анатольевич", "Козлов Дмитрий Викторович",
                "Аппендицит", "Стационарное", new DateTime(2024, 12, 20), "Нет", new DateTime(2024, 12, 10)));

            diagnoses.Add(new DiagnosisInfo(7, "Васнецова Елена Владимировна", "Федоров Сергей Анатольевич",
                "Перелом бедра", "Стационарное", new DateTime(2025, 3, 1), "Да", new DateTime(2024, 11, 5)));

            diagnoses.Add(new DiagnosisInfo(8, "Григорьев Андрей Борисович", "Николаева Ольга Игоревна",
                "Язва желудка", "Стационарное", new DateTime(2025, 2, 28), "Да", new DateTime(2024, 10, 15)));

            diagnoses.Add(new DiagnosisInfo(9, "Морозова Татьяна Леонидовна", "Кузнецова Ирина Александровна",
                "Гайморит", "Стационарное", new DateTime(2025, 1, 15), "Нет", new DateTime(2024, 12, 1)));

            // 3. Хронические заболевания
            diagnoses.Add(new DiagnosisInfo(10, "Белов Павел Сергеевич", "Орлова Светлана Дмитриевна",
                "Бронхиальная астма", "Амбулаторное", new DateTime(2026, 12, 31), "Да", new DateTime(2023, 5, 10)));

            diagnoses.Add(new DiagnosisInfo(11, "Кузнецова Ирина Александровна", "Павлова Наталья Михайловна",
                "Миопия высокой степени", "Амбулаторное", new DateTime(2025, 12, 31), "Нет", new DateTime(2024, 8, 20)));

            diagnoses.Add(new DiagnosisInfo(12, "Семенов Виктор Геннадьевич", "Жуков Максим Ильич",
                "Сахарный диабет 2 типа", "Амбулаторное", new DateTime(2025, 12, 31), "Да", new DateTime(2024, 2, 10)));

            // 4. Разные заболевания разных специальностей
            diagnoses.Add(new DiagnosisInfo(13, "Павлова Наталья Михайловна", "Белов Павел Сергеевич",
                "Вегето-сосудистая дистония", "Амбулаторное", new DateTime(2025, 5, 30), "Нет", new DateTime(2024, 9, 15)));

            diagnoses.Add(new DiagnosisInfo(14, "Киселев Артем Олегович", "Григорьев Андрей Борисович",
                "Ишемическая болезнь сердца", "Стационарное", new DateTime(2025, 4, 15), "Да", new DateTime(2024, 10, 5)));

            diagnoses.Add(new DiagnosisInfo(15, "Орлова Светлана Дмитриевна", "Семенов Виктор Геннадьевич",
                "Острый бронхит", "Амбулаторное", new DateTime(2024, 12, 25), "Нет", new DateTime(2024, 12, 10)));

            // 5. Травмы
            diagnoses.Add(new DiagnosisInfo(16, "Жуков Максим Ильич", "Федоров Сергей Анатольевич",
                "Черепно-мозговая травма", "Стационарное", new DateTime(2025, 6, 30), "Да", new DateTime(2024, 11, 20)));

            diagnoses.Add(new DiagnosisInfo(17, "Романова Екатерина Алексеевна", "Сидоров Алексей Петрович",
                "Растяжение связок", "Амбулаторное", new DateTime(2025, 1, 10), "Нет", new DateTime(2024, 12, 5)));

            diagnoses.Add(new DiagnosisInfo(18, "Дмитриев Константин Викторович", "Морозова Татьяна Леонидовна",
                "Остеохондроз", "Амбулаторное", new DateTime(2025, 12, 31), "Да", new DateTime(2024, 7, 15)));

            // 6. Детские заболевания
            diagnoses.Add(new DiagnosisInfo(19, "Андреев Михаил Олегович", "Орлова Светлана Дмитриевна",
                "Ветряная оспа", "Амбулаторное", new DateTime(2024, 12, 20), "Нет", new DateTime(2024, 12, 1)));

            diagnoses.Add(new DiagnosisInfo(20, "Сергеева Марина Владимировна", "Киселев Артем Олегович",
                "Панкреатит", "Стационарное", new DateTime(2025, 2, 15), "Да", new DateTime(2024, 11, 25)));

            // 7. Разные сроки лечения
            diagnoses.Add(new DiagnosisInfo(21, "Иванов Иван Иванович", "Васнецова Елена Владимировна",
                "Гипертония", "Амбулаторное", new DateTime(2025, 12, 31), "Да", new DateTime(2023, 1, 10)));

            diagnoses.Add(new DiagnosisInfo(22, "Петрова Анна Сергеевна", "Кузнецова Ирина Александровна",
                "Отит", "Амбулаторное", new DateTime(2025, 1, 5), "Нет", new DateTime(2024, 12, 15)));

            RefreshDataGrid();
        }

        private void RefreshDataGrid()
        {
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = diagnoses;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Функция добавления диагноза", "Информация",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                var result = MessageBox.Show("Удалить выбранные диагнозы?", "Подтверждение",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    var selectedRows = dataGridView1.SelectedRows.Cast<DataGridViewRow>().ToList();
                    var idsToRemove = selectedRows.Select(row => ((DiagnosisInfo)row.DataBoundItem).ID).ToList();

                    diagnoses.RemoveAll(d => idsToRemove.Contains(d.ID));
                    RefreshDataGrid();

                    MessageBox.Show($"Удалено {selectedRows.Count} диагнозов", "Информация",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Выберите диагнозы для удаления", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Обработка клика по ячейке
        }

        private class DiagnosisInfo
        {
            public int ID { get; set; }
            public string Пациент { get; set; }
            public string Врач { get; set; }
            public string Диагноз { get; set; }
            public string Лечение { get; set; }
            public DateTime Срок_лечения { get; set; }
            public string Учёт { get; set; }
            public DateTime Дата_начала { get; set; }

            public DiagnosisInfo(int id, string patient, string doctor, string diagnosis,
                               string treatment, DateTime term, string account, DateTime startDate)
            {
                ID = id;
                Пациент = patient;
                Врач = doctor;
                Диагноз = diagnosis;
                Лечение = treatment;
                Срок_лечения = term;
                Учёт = account;
                Дата_начала = startDate;
            }
        }
    }
}
