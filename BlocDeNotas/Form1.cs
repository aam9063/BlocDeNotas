using System;
using System.Drawing;
using System.Windows.Forms;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;

namespace BlocDeNotas
{
    public partial class Form1 : Form
    {
        private bool isTextChanged = false;


        public Form1()
        {
            InitializeComponent(); // Inicializa los componentes del formulario
            this.FormClosing += Form1_FormClosing; // Asignamos el evento FormClosing al formulario
            richTextBox1.Dock = DockStyle.Fill; // Ajusta el control RichTextBox al tamaño del formulario

            // Creamos el menú contextual
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();

            // Creamos y agregamos elementos de menú
            ToolStripMenuItem cortarItem = new ToolStripMenuItem("Cortar");
            cortarItem.Click += cortarToolStripMenuItem_Click; // Asignamos el evento Click al método que maneja la acción de cortar
            contextMenuStrip.Items.Add(cortarItem);

            ToolStripMenuItem copiarItem = new ToolStripMenuItem("Copiar");
            copiarItem.Click += copiarToolStripMenuItem_Click; // Asignamos el evento Click al método que maneja la acción de copiar
            contextMenuStrip.Items.Add(copiarItem);

            ToolStripMenuItem pegarItem = new ToolStripMenuItem("Pegar");
            pegarItem.Click += pegarToolStripMenuItem_Click; // Asignamos el evento Click al método que maneja la acción de pegar
            contextMenuStrip.Items.Add(pegarItem);

            // Asignamos el menú contextual al RichTextBox
            richTextBox1.ContextMenuStrip = contextMenuStrip;

            // Shortcut Keys
            guardarToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.G; 
            guardarComoToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.H;
            abrirToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.A;
            nuevoToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.N;
            cortarToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.X;
            copiarToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.C;
            pegarToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.V;
            deshacerToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Z;
            rehacerToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Y;
            aumentarTamañoToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Add;
            disminuirTamañoToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Subtract;
            negritaToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.B;
            cursivaToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.I;
            subrayarToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.U;
            fuenteToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.F;
            colorDeFuenteToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.F;
            colorDeFondoToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.B;
            seleccionarTodoToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.E;

        }

        // Eventos de los menús
        private void abrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog(); // Crea un nuevo cuadro de diálogo para abrir archivos
            openFileDialog.Filter = "Rich Text Format (*.rtf)|*.rtf"; // Establece el filtro para mostrar solo archivos .rtf
            if (openFileDialog.ShowDialog() == DialogResult.OK) // Si el usuario selecciona un archivo y hace clic en el botón Aceptar
            {
                richTextBox1.LoadFile(openFileDialog.FileName, RichTextBoxStreamType.RichText); // Carga el archivo seleccionado en el control RichTextBox
            }
        }


        private void guardarToolStripMenuItem_Click(object sender, EventArgs e) // Método para guardar el archivo
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog(); // Crea un nuevo cuadro de diálogo para guardar archivos
            saveFileDialog.Filter = "Rich Text Format (*.rtf)|*.rtf";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                richTextBox1.SaveFile(saveFileDialog.FileName, RichTextBoxStreamType.RichText);
            }

            isTextChanged = false; // Cambia el estado de isTextChanged a false para indicar que no hay cambios en el archivo

        }


        private void guardarComoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog(); // Crea un nuevo cuadro de diálogo para guardar archivos
            saveFileDialog.Filter = "Text Files (*.txt)|*.txt|Rich Text Format (*.rtf)|*.rtf|Word Document (*.docx)|*.docx"; // Establece el filtro para mostrar archivos .txt, .rtf y .docx
            if (saveFileDialog.ShowDialog() == DialogResult.OK) // Si el usuario selecciona un archivo y hace clic en el botón Aceptar
            {
                switch (System.IO.Path.GetExtension(saveFileDialog.FileName).ToLower()) // Obtiene la extensión del archivo seleccionado
                {
                    case ".txt":
                        richTextBox1.SaveFile(saveFileDialog.FileName, RichTextBoxStreamType.PlainText);
                        break;
                    case ".rtf":
                        richTextBox1.SaveFile(saveFileDialog.FileName, RichTextBoxStreamType.RichText);
                        break;
                    case ".docx":
                        // Crea un nuevo documento de Word
                        using (var package = WordprocessingDocument.Create(saveFileDialog.FileName, WordprocessingDocumentType.Document)) 
                        {
                            package.AddMainDocumentPart();
                            package.MainDocumentPart.Document = new Document(
                                new DocumentFormat.OpenXml.Wordprocessing.Body(
                                    new Paragraph(
                                        new Run(
                                            new DocumentFormat.OpenXml.Drawing.Text(richTextBox1.Text)))));
                        }
                        break;
                }
            }

            isTextChanged = false; // Cambia el estado de isTextChanged a false para indicar que no hay cambios en el archivo
        }


        private void deshacerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.CanUndo)
            {
                richTextBox1.Undo(); // Deshace la última acción realizada en el control RichTextBox
                // Limpia el estado de deshacer para evitar deshacer acciones que no son de edición de texto
                richTextBox1.ClearUndo();
            }
        }

        private void rehacerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.CanRedo) // Si se puede rehacer la última acción
            {
                richTextBox1.Redo(); // Rehace la última acción realizada en el control RichTextBox
            }
        }

        private void cortarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.SelectedText != "") // Si hay texto seleccionado
            {
                richTextBox1.Cut(); // Corta el texto seleccionado y lo copia al portapapeles
            }
        }

        private void copiarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.SelectedText != "") // Si hay texto seleccionado
            {
                richTextBox1.Copy(); // Copia el texto seleccionado al portapapeles
            }
        }

        private void pegarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Paste();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            isTextChanged = true; // Cambia el estado de isTextChanged a true para indicar que hay cambios en el archivo
        }

       

        private void nuevoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear(); // Limpia el control RichTextBox
        }

        private void aumentarTamañoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Aumenta el tamaño de la fuente en 1
            richTextBox1.SelectionFont = new System.Drawing.Font(richTextBox1.SelectionFont.FontFamily, richTextBox1.SelectionFont.Size + 1);
        }

        private void disminuirTamañoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Disminuye el tamaño de la fuente en 1
            richTextBox1.SelectionFont = new System.Drawing.Font(richTextBox1.SelectionFont.FontFamily, richTextBox1.SelectionFont.Size - 1);
        }

        private void negritaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Cambia el estilo de la fuente a negrita
            richTextBox1.SelectionFont = new System.Drawing.Font(richTextBox1.SelectionFont, richTextBox1.SelectionFont.Style ^ FontStyle.Bold);
        }

        private void cursivaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Cambia el estilo de la fuente a cursiva
            richTextBox1.SelectionFont = new System.Drawing.Font(richTextBox1.SelectionFont, richTextBox1.SelectionFont.Style ^ FontStyle.Italic);
        }

        private void subrayarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Cambia el estilo de la fuente a subrayado
            richTextBox1.SelectionFont = new System.Drawing.Font(richTextBox1.SelectionFont, richTextBox1.SelectionFont.Style ^ FontStyle.Underline);
        }

        
        private void fuenteToolStripMenuItem_Click(object sender, EventArgs e) // Método para cambiar la fuente del texto
        {
            FontDialog fontDialog = new FontDialog(); // Crea un nuevo cuadro de diálogo para seleccionar la fuente
            if (fontDialog.ShowDialog() == DialogResult.OK) // Si el usuario selecciona una fuente y hace clic en el botón Aceptar
            {
                richTextBox1.SelectionFont = fontDialog.Font; // Cambia la fuente del texto seleccionado
            }
        }

        private void colorDeFuenteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                richTextBox1.SelectionColor = colorDialog.Color;
            }
        }

        private void colorDeFondoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                richTextBox1.SelectionBackColor = colorDialog.Color;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) // Método para cerrar la aplicación
        {
            if (isTextChanged)
            {
                var result = MessageBox.Show("¿Quieres guardar los cambios?", "Bloc de notas", MessageBoxButtons.YesNoCancel);
                switch (result)
                {
                    case DialogResult.Yes:
                        // Aquí puedes llamar al método para guardar el archivo
                        guardarToolStripMenuItem_Click(sender, e);
                        break;
                    case DialogResult.No:
                        // No hacer nada, simplemente cerrar la aplicación
                        break;
                    case DialogResult.Cancel:
                        // Cancelar el cierre de la aplicación
                        e.Cancel = true;
                        break;
                }
            }
        }

        private void seleccionarTodoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectAll(); // Selecciona todo el texto en el control RichTextBox
        }

        private void cortarToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void copiarToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void pegarToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }
    }
}
