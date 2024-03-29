﻿using System;
using System.Text.RegularExpressions;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using System.Drawing;

namespace Form2
{
    public partial class FormRender : System.Windows.Forms.Form
    {
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBox6;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox comboBox3;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox textBox9;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox textBox8;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.MaskedTextBox maskedTextBox1;
        private System.Windows.Forms.ComboBox comboBox4;

        private Regex reg = new Regex(@"^-?\d+[,]?\d*$");
        private ExternalCommandData commandData;
        private readonly Document doc;
        private List<Element> lights_on;
        private readonly List<List<Mesh>> allMesh;
        private readonly List<List<Material>> materials;
        private readonly List<XYZ> listCam;
        private List<MyObject> objects = new List<MyObject>();
        private int width, height, nSamples = 0, ray_depth = 0;

        public Regex Reg { get => reg; set => reg = value; }
        public ExternalCommandData CommandData { get => commandData; set => commandData = value; }

        public FormRender(ExternalCommandData commandData, List<Element> lights, List<List<Mesh>> allMesh, List<List<Material>> materials, List<XYZ> listCam, Document doc)
        {
            this.CommandData = commandData;
            this.lights_on = lights;
            this.allMesh = allMesh;
            this.materials = materials;
            this.listCam = listCam;
            this.doc = doc;

            GetObjects();

            InitializeComponent();

        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRender));
            this.button2 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.maskedTextBox1 = new System.Windows.Forms.MaskedTextBox();
            this.textBox9 = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.textBox8 = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.comboBox3 = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.comboBox4 = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(197, 377);
            this.button2.Name = "button2";
            this.button2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 0;
            this.button2.Text = "Renderizar";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.Button2_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Altura";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(153, 17);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Largura";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBox1);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.textBox3);
            this.groupBox1.Controls.Add(this.textBox2);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(260, 74);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Detalhes da imagem";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Baixa",
            "Média",
            "Alta"});
            this.comboBox1.Location = new System.Drawing.Point(91, 42);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 21);
            this.comboBox1.TabIndex = 5;
            this.comboBox1.SelectedIndex = 0;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 47);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Qualidade";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(206, 14);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(44, 20);
            this.textBox3.TabIndex = 3;
            this.textBox3.Text = "200";
            this.textBox3.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox1_KeyPress);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(46, 17);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(44, 20);
            this.textBox2.TabIndex = 2;
            this.textBox2.Text = "200";
            this.textBox2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox1_KeyPress);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBox6);
            this.groupBox2.Controls.Add(this.textBox5);
            this.groupBox2.Controls.Add(this.textBox4);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Location = new System.Drawing.Point(12, 92);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(260, 69);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Câmera";
            // 
            // textBox6
            // 
            this.textBox6.Location = new System.Drawing.Point(91, 39);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(44, 20);
            this.textBox6.TabIndex = 8;
            this.textBox6.Text = "50";
            this.textBox6.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox1_KeyPress_double);
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(206, 13);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(44, 20);
            this.textBox5.TabIndex = 7;
            this.textBox5.Text = "0";
            this.textBox5.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox1_KeyPress_double);
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(91, 13);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(44, 20);
            this.textBox4.TabIndex = 6;
            this.textBox4.Text = "45";
            this.textBox4.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox1_KeyPress_double);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 42);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(77, 13);
            this.label8.TabIndex = 2;
            this.label8.Text = "Distancia focal";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(153, 16);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(47, 13);
            this.label7.TabIndex = 1;
            this.label7.Text = "Abertura";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 16);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(83, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Campo de visão";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.maskedTextBox1);
            this.groupBox3.Controls.Add(this.textBox9);
            this.groupBox3.Controls.Add(this.label14);
            this.groupBox3.Controls.Add(this.textBox8);
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.comboBox3);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.button3);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Location = new System.Drawing.Point(12, 167);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(260, 146);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Iluminação";
            // 
            // maskedTextBox1
            // 
            this.maskedTextBox1.Location = new System.Drawing.Point(91, 74);
            this.maskedTextBox1.Mask = "00/00/0000 90:00";
            this.maskedTextBox1.Name = "maskedTextBox1";
            this.maskedTextBox1.Size = new System.Drawing.Size(123, 20);
            this.maskedTextBox1.TabIndex = 9;
            this.maskedTextBox1.Text = "110320190800";
            this.maskedTextBox1.ValidatingType = typeof(System.DateTime);
            // 
            // textBox9
            // 
            this.textBox9.Location = new System.Drawing.Point(173, 111);
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new System.Drawing.Size(44, 20);
            this.textBox9.TabIndex = 16;
            this.textBox9.Text = "-35,2";
            this.textBox9.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox9_KeyPress);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(117, 114);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(54, 13);
            this.label14.TabIndex = 15;
            this.label14.Text = "Longitude";
            // 
            // textBox8
            // 
            this.textBox8.Location = new System.Drawing.Point(63, 111);
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new System.Drawing.Size(44, 20);
            this.textBox8.TabIndex = 14;
            this.textBox8.Text = "-5,7";
            this.textBox8.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox8_KeyPress);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(15, 114);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(45, 13);
            this.label13.TabIndex = 13;
            this.label13.Text = "Latitude";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(8, 77);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(51, 13);
            this.label11.TabIndex = 11;
            this.label11.Text = "Dia/Hora";
            // 
            // comboBox3
            // 
            this.comboBox3.FormattingEnabled = true;
            this.comboBox3.Items.AddRange(new object[] {
            "Céu Aberto",
            "Parcialmente Nublado",
            "Nublado"});
            this.comboBox3.Location = new System.Drawing.Point(91, 45);
            this.comboBox3.Name = "comboBox3";
            this.comboBox3.Size = new System.Drawing.Size(121, 21);
            this.comboBox3.TabIndex = 9;
            this.comboBox3.SelectedIndex = 0;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(8, 48);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(65, 13);
            this.label10.TabIndex = 8;
            this.label10.Text = "Tipo de Céu";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(91, 13);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(126, 21);
            this.button3.TabIndex = 7;
            this.button3.Text = "Selecionar";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.Button3_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(8, 17);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(43, 13);
            this.label9.TabIndex = 6;
            this.label9.Text = "Artificial";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(18, 332);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(92, 13);
            this.label15.TabIndex = 6;
            this.label15.Text = "Imagem de Fundo";
            // 
            // comboBox4
            // 
            this.comboBox4.FormattingEnabled = true;
            this.comboBox4.Items.AddRange(new object[] {
            "Céu",
            "Cor",
            "Imagem"});
            this.comboBox4.Location = new System.Drawing.Point(128, 329);
            this.comboBox4.Name = "comboBox4";
            this.comboBox4.Size = new System.Drawing.Size(121, 21);
            this.comboBox4.TabIndex = 17;
            this.comboBox4.SelectedIndex = 0;
            // 
            // FormRender
            // 
            this.ClientSize = new System.Drawing.Size(280, 412);
            this.Controls.Add(this.comboBox4);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormRender";
            this.Text = "PixeR";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void TextBox1_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            // Verify that the pressed key isn't CTRL or any non-numeric digit
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void TextBox1_KeyPress_double(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            // Verify that the pressed key isn't CTRL or any non-numeric digit
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != ','))
            {
                e.Handled = true;
            }

            // If you want, you can allow decimal (float) numbers
            System.Windows.Forms.TextBox a;
            a = sender as System.Windows.Forms.TextBox;
            if ((e.KeyChar == ',') && (a.Text.IndexOf(',') > -1))
            {
                e.Handled = true;
            }
        }

        private void TextBox8_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
            {
                return;
            }

            if (!Reg.IsMatch(textBox8.Text.Insert(textBox8.SelectionStart, e.KeyChar.ToString()) + "1"))
            {
                e.Handled = true;
                return;
            }
        }

        private void TextBox9_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar)) return;
            if (!Reg.IsMatch(textBox9.Text.Insert(textBox9.SelectionStart, e.KeyChar.ToString()) + "1"))
            {
                e.Handled = true;
                return;
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            Form3.FormLight fl = new Form3.FormLight(CommandData, lights_on);
            fl.ShowDialog();
            lights_on = fl.GetLights();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (textBox2.Text != "")
            {
                if (textBox3.Text != "")
                {
                    if (comboBox1.SelectedIndex != -1)
                    {
                        if (textBox4.Text != "")
                        {
                            if (textBox5.Text != "")
                            {
                                if (textBox6.Text != "")
                                {
                                    if (comboBox3.SelectedIndex != -1)
                                    {
                                        string[] diaMes = maskedTextBox1.Text.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

                                        string[] AnoHora = diaMes[2].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                                        string[] horaMin = AnoHora[1].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);


                                        if (diaMes.Length == 3 && AnoHora.Length == 2 && horaMin.Length == 2)
                                        {

                                            if (textBox8.Text != "")
                                            {
                                                if (textBox9.Text != "")
                                                {
                                                    if (comboBox4.SelectedIndex != -1)
                                                    {
                                                        //TaskDialog.Show("PixeR", textBox2.Text + " " + textBox3.Text + " " + comboBox1.Text + " " + textBox4.Text + " " + textBox5.Text + " " + textBox6 + " " + comboBox3.Text + " -" + maskedTextBox1.Text + "- " + textBox8.Text + " " + textBox9.Text + " " + comboBox4.Text);

                                                        int day = Convert.ToInt32(diaMes[0]);
                                                        int month = Convert.ToInt32(diaMes[1]);
                                                        int year = Convert.ToInt32(AnoHora[0]);
                                                        int hour = Convert.ToInt32(horaMin[0]);
                                                        int min = Convert.ToInt32(horaMin[1]);

                                                        Psa.Psa positionSun = new Psa.Psa(GetLatitude(), Getlongitude(), day, month, year, hour, min);
                                                        XYZ sun = positionSun.GetPosition();

                                                        Bitmap image = GetImage(sun);

                                                        Form4.FormsImage fi = new Form4.FormsImage(image);
                                                        fi.ShowDialog();
                                                    }
                                                    else
                                                    {
                                                        TaskDialog.Show("PixeR", "Selecione o fundo");
                                                    }
                                                }
                                                else
                                                {
                                                    TaskDialog.Show("PixeR", "Selecione a altitude");
                                                }
                                            }
                                            else
                                            {
                                                TaskDialog.Show("PixeR", "Selecione a Latitude");
                                            }
                                        }
                                        else
                                        {
                                            TaskDialog.Show("P", "Selecione o dia e hora");
                                        }
                                    }
                                    else
                                    {
                                        TaskDialog.Show("PixeR", "Selecione o tipo de céu");
                                    }
                                }
                                else
                                {
                                    TaskDialog.Show("PixeR", "Selecione o campo distância focal");
                                }
                            }
                            else
                            {
                                TaskDialog.Show("PixeR", "Selecione o campo abertura da câmera");
                            }
                        }
                        else
                        {
                            TaskDialog.Show("Pixer", "Selecione o campo de visão da câmera!");
                        }
                    }
                    else
                    {
                        TaskDialog.Show("PixeR", "Selecione a qualidade");
                    }
                }
                else
                {
                    TaskDialog.Show("PixeR", "Preencha o campo largura");
                }
            }
            else
            {
                TaskDialog.Show("PixeR", "Preencha o campo altura");
            }
        }

        public int GetAltura()
        {
            return Convert.ToInt32(textBox2.Text);
        }

        public int GetLargura()
        {
            return Convert.ToInt32(textBox3.Text);
        }

        public String GetQualidade()
        {
            return comboBox1.Text;
        }

        public Double GetCampoVisao()
        {
            return Convert.ToDouble(textBox4.Text);
        }

        public Double GetAbertura()
        {
            return Convert.ToDouble(textBox5.Text);
        }

        public Double GetDistFocal()
        {
            return Convert.ToDouble(textBox6.Text);
        }

        public String GetCeu()
        {
            return comboBox3.Text;
        }

        public String GetDataHora()
        {
            return maskedTextBox1.Text;
        }

        public Double GetLatitude()
        {
            return Convert.ToDouble(textBox8.Text);
        }

        public Double Getlongitude()
        {
            return Convert.ToDouble(textBox9.Text);
        }

        public string GetBackGround()
        {
            return comboBox4.Text;
        }

        public void GetObjects()
        {
            //String mensagem = "";
            for (int i = 0; i < allMesh.Count; i++)
            {
                //mensagem += "Objeto\n\n";
                List<Triangle> triangles = new List<Triangle>();

                for (int j = 0; j < allMesh[i].Count; j++)
                {
                    //Get Material
                    double red = Convert.ToDouble(materials[i][j].Color.Red) / 255;
                    double green = Convert.ToDouble(materials[i][j].Color.Green) / 255;
                    double blue = Convert.ToDouble(materials[i][j].Color.Blue) / 255;

                    XYZ kd = new XYZ(red, green, blue);

                    MyMaterial mat = new Lambertian(new ConstantTexture(kd));
                    double ri =1;
                    mat = new DIalectrisMaterial(new ConstantTexture(kd), ri);

                    for (int k = 0; k < allMesh[i][j].NumTriangles; k++)
                    {
                        //Get points 
                        MeshTriangle triangle = allMesh[i][j].get_Triangle(k);

                        XYZ v1 = new XYZ(triangle.get_Vertex(0).X, -triangle.get_Vertex(0).Z, -triangle.get_Vertex(0).Y);
                        XYZ v2 = new XYZ(triangle.get_Vertex(1).X, -triangle.get_Vertex(1).Z, -triangle.get_Vertex(1).Y);
                        XYZ v3 = new XYZ(triangle.get_Vertex(2).X, -triangle.get_Vertex(2).Z, -triangle.get_Vertex(2).Y);


                        triangles.Add(new Triangle(mat, v1, v2, v3));
                    }

                }
                objects.Add(new MyMash(triangles));
            }
        }

        public void GetQuality(string quality)
        {
            if (quality == "Baixa")
            {
                nSamples = 10; ray_depth = 50;
            }
            else if (quality == "Média")
            {
                nSamples = 50; ray_depth = 50;

            }
            else if (quality == "Alta")
            {
                nSamples = 100; ray_depth = 100;
            }
        }

        public BackGround GetBackGround(string backGround)
        {
            XYZ topleft;
            XYZ topRight;
            XYZ bottonLeft;
            XYZ bottonRight;
            if (backGround == "Céu")
            {
                topleft = new XYZ(1, 1, 1);
                topRight = new XYZ(1, 1, 1);
                bottonLeft = new XYZ(0, 0, 0);
                bottonRight = new XYZ(0, 0, 0);
            }
            else
            {
                topleft = new XYZ(1, 1, 1);
                topRight = new XYZ(1, 1, 1);
                bottonLeft = new XYZ(1, 1, 1);
                bottonRight = new XYZ(1, 1, 1);
            }
            return new BackGround(topleft, topRight, bottonLeft, bottonRight);
        }

        public PerspectiveCamera GetCamera()
        {
            XYZ eye = new XYZ(listCam[0].X, -listCam[0].Z, -listCam[0].Y);
            XYZ direction = new XYZ(listCam[1].X, -listCam[0].Z, -listCam[1].Y);
            return new PerspectiveCamera(eye, direction, new XYZ(0, 1, 0), GetCampoVisao(), width / height, GetAbertura(), GetDistFocal());
        }
        public Bitmap GetImage(XYZ posicaoSol)
        {
            width = GetLargura();
            height = GetAltura();
            GetQuality(GetQualidade());

            double t_min = 0.00001, t_max = double.PositiveInfinity;

            BackGround bg = GetBackGround(GetBackGround());

            XYZ luzAmbiente = new XYZ(0.1, 0.1, 0.1);

            List<MyLight> luzes = new List<MyLight>();
            //adiciona o sol
            PointLight sol = new PointLight(posicaoSol, new XYZ(1, 1, 0));
            luzes.Add(sol);
            //adiciona luzes artificiais

            Scene world = new Scene(objects, luzes, bg, luzAmbiente);

            Shader shader = new LambertianShader(world);

            PerspectiveCamera cam = GetCamera();

            Raytrace raytrace = new Raytrace();
            Bitmap img = raytrace.Render(cam, world, shader, width, height, nSamples, ray_depth, t_min, t_max);

            return img;
        }
    }
}