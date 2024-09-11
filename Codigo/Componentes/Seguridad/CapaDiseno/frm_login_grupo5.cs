﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapaDatos;
using CapaLogica;

namespace CapaDiseno
{
    public partial class frm_login_grupo5 : Form
    {
        public string bExito = "Pendiente";

        public frm_login_grupo5()
        {
            InitializeComponent();
            Txt_clave.UseSystemPasswordChar = true;
        }

        string nombreUsuario = "";
        public string obtenerNombreUsuario()
        {
            nombreUsuario = Txt_usuario.Text;
            return nombreUsuario;
        }

        //Fernando García 0901-21-581//


        private void Frm_login_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }


        private void Btn_entrar_Click_1(object sender, EventArgs e)
        {
            ProcedimientoLogin_grupo5 procedimientoLogin = new ProcedimientoLogin_grupo5();

            if (Txt_usuario.Text.Trim() == "")
            {
                MessageBox.Show("Debe ingresar un usuario");
            }
            else
            {
                if (Txt_clave.Text.Trim() == "")
                {
                    MessageBox.Show("Debe ingresar su contraseña");
                }
                else
                {
                    try
                    {
                        bool bExisteUsuario = procedimientoLogin.llamarProcedimiento(Txt_usuario.Text, Txt_clave.Text);

                        if (bExisteUsuario)
                        {

                            // Ocultar el formulario de login en lugar de cerrarlo
                            this.Hide();

                            sentencia_grupo5 s = new sentencia_grupo5();
                            s.insertarBitacora(Txt_usuario.Text.Trim(), "Se logeo al sistema", "Login");

                            // Pasa el nombre de usuario al constructor de MDI_Seguridad
                            MDI_Seguridad formMDI = new MDI_Seguridad(Txt_usuario.Text);
                            formMDI.Show();

                        }
                        else
                        {
                            MessageBox.Show("Usuario o Contraseña Incorrecta", "Verificacion de Login", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        MessageBox.Show("No Conecto La Base de Datos", "Verificar Conexión", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
        }

        private void Btn_cancelar_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Cb_ver_password_CheckedChanged_1(object sender, EventArgs e)
        {
            if (Cb_ver_password.Checked == false)
            {
                Txt_clave.UseSystemPasswordChar = true;
            }
            else
            {
                Txt_clave.UseSystemPasswordChar = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, "C:\\Ayuda_Seguridad\\" + "ayudaLogin.chm", "login.html");
        }

        private void Btn_olvidocontrasenia_Click_1(object sender, EventArgs e)
        {
            // Mostrar el formulario de entrada con una pregunta
            using (var frm_cambio_contrasenia = new frm_cambio_contrasenia("Por favor, ingresa tu usuario:"))
            {
                if (frm_cambio_contrasenia.ShowDialog() == DialogResult.OK)
                {
                    // Obtener el resultado del formulario
                    string nombre = frm_cambio_contrasenia.Resultado;
                    MessageBox.Show("El nombre ingresado es: " + nombre);
                }
                /*else
                {
                    MessageBox.Show("Entrada cancelada.");
                }*/
            }
        }
    }
}
