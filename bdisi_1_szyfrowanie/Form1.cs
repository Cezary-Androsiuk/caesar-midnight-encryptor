using System.Diagnostics;

namespace bdisi_1_szyfrowanie
{
    public partial class Form1 : Form
    {
        enum EncryptionMethod
        {
            None = 0,
            Cezar,
            Midnight
        }
        EncryptionMethod method;

        const String charSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789 !@#$%^&*()_+-=[]{}|\\;:\"',.<>?/~`\n";
        //const String charSet = "0123456789";

        const String moduloLabelTextFormat = "% {0} = {1}";

        const int midnightGrow = 1;

        private void updateEncryptionMethod()
        {
            String text = comboBox1.Text;

            if (text == "Cezar")
                this.method = EncryptionMethod.Cezar;
            else if (text == "Midnight")
                this.method = EncryptionMethod.Midnight;
            else
                this.method = EncryptionMethod.None;
        }

        private int validateKey(int key, int setSize = 0)
        {
            int validKey;
            if (setSize <= 0) /// if was not set, then use default
                setSize = charSet.Length;

            if (key >= 0)
            {
                validKey = key % setSize;
            }
            else
            {
                validKey = key % setSize;

                if (validKey < 0) /// handle case when key == -setSize
                    validKey += setSize;
            }
            return validKey;
        }

        private String encryptCezar(String content, int key)
        {
            String encrypted = String.Empty;
            foreach (char c in content)
            {
                /// change letter to different from charSet, based on a key value
                int index = charSet.IndexOf(c);
                if (index == -1)
                {
                    label6.Text = String.Format(
                        "Text contain '{0}' character which is not in char set", c);
                    return "";
                }

                index = validateKey(index + key); /// shift index by key
                //encrypted += String.Format("{0}=>{1}, ", index, newindex);
                encrypted += charSet[index];
            }
            return encrypted;
        }

        private String decryptCezar(String content, int key)
        {
            return encryptCezar(content, -key);
        }

        private String encryptMidnight(String content, int key, int grow = 0)
        {
            String encrypted = String.Empty;

            if (grow == 0) // if not set then use default
                grow = midnightGrow;

            foreach (char c in content)
            {
                /// change letter to different from charSet, based on a key value
                int index = charSet.IndexOf(c);
                if (index == -1)
                {
                    label6.Text = String.Format(
                        "Text contain '{0}' character which is not in char set", c);
                    return "";
                }

                index = validateKey(index + key); /// shift index by key
                //encrypted += String.Format("{0}=>{1}, ", index, newindex);
                encrypted += charSet[index];

                key += grow;
            }
            return encrypted;
        }

        private String decryptMidnight(String content, int key, int grow = 0)
        {
            String encrypted = String.Empty;

            if (grow == 0) // if not set then use default
                grow = midnightGrow;

            foreach (char c in content)
            {
                /// change letter to different from charSet, based on a key value
                int index = charSet.IndexOf(c);
                if (index == -1)
                {
                    label6.Text = String.Format(
                        "Text contain '{0}' character which is not in char set", c);
                    return "";
                }

                index = validateKey(index - key); /// shift index by key
                //encrypted += String.Format("{0}=>{1}, ", index, newindex);
                encrypted += charSet[index];

                key += grow;
            }
            return encrypted;
        }

        public Form1()
        {
            InitializeComponent();

            label6.Text = "";
            int modNumber = 0;
            if (textBox1.Text != "")
                modNumber = int.Parse(textBox1.Text);
            label7.Text = String.Format(moduloLabelTextFormat, charSet.Length, modNumber);

            //textBox1.Text = "123";
            //comboBox1.Text = EncryptionMethod.Midnight.ToString();
            //textBox2.Text = "My text was awesome 1234 /";

            /// tests
            //012345678901234567890123456789
            Debug.Assert(validateKey(3, 10) == 3);
            Debug.Assert(validateKey(10, 10) == 0);
            Debug.Assert(validateKey(13, 10) == 3);
            Debug.Assert(validateKey(20, 10) == 0);
            Debug.Assert(validateKey(23, 10) == 3);
            Debug.Assert(validateKey(30, 10) == 0);
            Debug.Assert(validateKey(33, 10) == 3);
            Debug.Assert(validateKey(40, 10) == 0);

            Debug.Assert(validateKey(1, 10) == 1);
            Debug.Assert(validateKey(0, 10) == 0);
            Debug.Assert(validateKey(-1, 10) == 9);

            Debug.Assert(validateKey(-3, 10) == 7);
            Debug.Assert(validateKey(-10, 10) == 0);
            Debug.Assert(validateKey(-13, 10) == 7);
            Debug.Assert(validateKey(-20, 10) == 0);
            Debug.Assert(validateKey(-23, 10) == 7);
            Debug.Assert(validateKey(-30, 10) == 0);
            Debug.Assert(validateKey(-33, 10) == 7);
            Debug.Assert(validateKey(-40, 10) == 0);

        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // Trim number from '0'
            int number = 0;
            String newText = textBox1.Text;
            if (newText != "")
                number = int.Parse(newText);
            int modNumber = number % charSet.Length;
            label7.Text = String.Format(moduloLabelTextFormat, charSet.Length, modNumber);
        }

        private bool prepareData()
        {
            /// Get current encryption method
            this.updateEncryptionMethod();
            if (this.method == EncryptionMethod.None)
            {
                label6.Text = "Please provide an encryption method.";
                return false;
            }

            /// Reset label
            label6.Text = "";

            /// If key was not set, then cancel
            if (textBox1.Text == "")
            {
                label6.Text = "Please provide an encryption key.";
                return false;
            }
            else if (textBox1.Text == "0")
            {
                label6.Text = "Please provide valid encryption key.";
                return false;
            }

            return true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            /// handle every invalid case
            if (!prepareData())
                return;

            /// always textBox1.Text > 0
            int key = int.Parse(textBox1.Text) % charSet.Length;

            /// none handled in prepare data
            if (this.method == EncryptionMethod.Cezar)
            {
                textBox3.Text = this.encryptCezar(textBox2.Text, key);
            }
            else if (this.method == EncryptionMethod.Midnight)
            {
                textBox3.Text = this.encryptMidnight(textBox2.Text, key);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            /// handle every invalid case
            if (!prepareData())
                return;

            /// always textBox1.Text > 0
            int key = int.Parse(textBox1.Text) % charSet.Length;

            /// none handled in prepare data
            if (this.method == EncryptionMethod.Cezar)
            {
                textBox4.Text = this.decryptCezar(textBox3.Text, key);

            }
            else if (this.method == EncryptionMethod.Midnight)
            {
                textBox4.Text = this.decryptMidnight(textBox3.Text, key);
            }
        }
    }
}
