namespace it9
{
    partial class MainMenu
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button btnPatients;
        private System.Windows.Forms.Button btnDoctors;
        private System.Windows.Forms.Button btnDiagnoses;
        private System.Windows.Forms.Button btnAmbulatoryPatients;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.btnPatients = new System.Windows.Forms.Button();
            this.btnDoctors = new System.Windows.Forms.Button();
            this.btnDiagnoses = new System.Windows.Forms.Button();
            this.btnAmbulatoryPatients = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();

            // btnPatients
            this.btnPatients.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnPatients.Location = new System.Drawing.Point(50, 100);
            this.btnPatients.Name = "btnPatients";
            this.btnPatients.Size = new System.Drawing.Size(200, 50);
            this.btnPatients.TabIndex = 0;
            this.btnPatients.Text = "Пациенты";
            this.btnPatients.UseVisualStyleBackColor = true;
            this.btnPatients.Click += new System.EventHandler(this.btnPatients_Click);

            // btnDoctors
            this.btnDoctors.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnDoctors.Location = new System.Drawing.Point(300, 100);
            this.btnDoctors.Name = "btnDoctors";
            this.btnDoctors.Size = new System.Drawing.Size(200, 50);
            this.btnDoctors.TabIndex = 1;
            this.btnDoctors.Text = "Врачи";
            this.btnDoctors.UseVisualStyleBackColor = true;
            this.btnDoctors.Click += new System.EventHandler(this.btnDoctors_Click);

            // btnDiagnoses
            this.btnDiagnoses.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnDiagnoses.Location = new System.Drawing.Point(550, 100);
            this.btnDiagnoses.Name = "btnDiagnoses";
            this.btnDiagnoses.Size = new System.Drawing.Size(200, 50);
            this.btnDiagnoses.TabIndex = 2;
            this.btnDiagnoses.Text = "Диагнозы";
            this.btnDiagnoses.UseVisualStyleBackColor = true;
            this.btnDiagnoses.Click += new System.EventHandler(this.btnDiagnoses_Click);

            // btnAmbulatoryPatients
            this.btnAmbulatoryPatients.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnAmbulatoryPatients.Location = new System.Drawing.Point(50, 200);
            this.btnAmbulatoryPatients.Name = "btnAmbulatoryPatients";
            this.btnAmbulatoryPatients.Size = new System.Drawing.Size(200, 50);
            this.btnAmbulatoryPatients.TabIndex = 3;
            this.btnAmbulatoryPatients.Text = "Амбулаторные пациенты";
            this.btnAmbulatoryPatients.UseVisualStyleBackColor = true;
            this.btnAmbulatoryPatients.Click += new System.EventHandler(this.btnAmbulatoryPatients_Click);

            // btnExit
            this.btnExit.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnExit.Location = new System.Drawing.Point(300, 200);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(200, 50);
            this.btnExit.TabIndex = 4;
            this.btnExit.Text = "Выход";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);

            // label1
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(250, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(300, 26);
            this.label1.TabIndex = 5;
            this.label1.Text = "Поликлиника - Главное меню";

            // panel1
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.btnPatients);
            this.panel1.Controls.Add(this.btnExit);
            this.panel1.Controls.Add(this.btnDoctors);
            this.panel1.Controls.Add(this.btnAmbulatoryPatients);
            this.panel1.Controls.Add(this.btnDiagnoses);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(800, 450);
            this.panel1.TabIndex = 6;

            // MainMenu
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.panel1);
            this.Name = "MainMenu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Поликлиника - Главное меню";
            this.Load += new System.EventHandler(this.MainMenu_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}
