using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace JsonDataGenerate
{
    public partial class Form1 : Form
    {
        private static string rowCount;
        private static string columnCount;
        private static Random random = new Random();


        public Form1()
        {
            InitializeComponent();
        }
        private void label1_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {

                List<Field> fields = new List<Field>();

                for (int i = 0; i < Convert.ToInt32(columnCount); i++)
                {
                    Field field = new Field();
                    var combo = this.Controls.Find("comboBox" + i.ToString(), true);
                    var columnName = this.Controls.Find("textboxDynamic" + i.ToString(), true);
                    var minVal = this.Controls.Find("textboxDynamicZoneFirst" + i.ToString(), true);
                    var maxVal = this.Controls.Find("textboxDynamicZoneSecond" + i.ToString(), true);

                    field.FieldName = columnName[0].Text;
                    if (combo[0].Text.ToLower() == "string")
                    {
                        field.FieldType = typeof(string);
                    }
                    else if (combo[0].Text.ToLower() == "ınteger")
                    {
                        field.FieldType = typeof(int);
                        field.Min = Convert.ToInt32(minVal[0].Text);
                        field.Max = Convert.ToInt32(maxVal[0].Text);
                    }
                    else if (combo[0].Text.ToLower() == "date")
                    {
                        field.FieldType = typeof(DateTime);
                    }
                    fields.Add(field);
                }

                var userName = System.Environment.UserName;
                List<object> objList = new List<object>();

                Random rnd = new Random();
                for (int i = 0; i < Convert.ToInt32(rowCount); i++)
                {
                    var myType = CompileResultType(fields);
                    object myObject = Activator.CreateInstance(myType);

                    foreach (PropertyInfo propertyInfo in myObject.GetType().GetProperties())
                    {
                        var prop = propertyInfo.ToString().Split(' ');
                        var propName = prop[1];

                        if (propertyInfo.ToString().Contains("Int"))
                        {
                            propertyInfo.SetValue(myObject, RandomNumber(rnd, fields[i].Min, fields[i].Max));
                        }
                        else if (propertyInfo.ToString().Contains("String"))
                        {
                            if (!propName.ToLower().Contains("name") && !propName.ToLower().Contains("surname"))
                                propertyInfo.SetValue(myObject, RandomString(10));
                            else
                            {
                                propertyInfo.SetValue(myObject, RandomStringForName(8) + " " + RandomStringForName(9));
                            }
                        }
                        else if (propertyInfo.ToString().Contains("Date"))
                        {
                            propertyInfo.SetValue(myObject, RandomDay(rnd));
                        }

                    }

                    objList.Add(myObject);
                }
                var path = @"C:\Users\" + userName + @"\json.txt";

                using (StreamWriter file = File.CreateText(path))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, objList);
                }

            }
            catch (Exception)
            {
                System.Windows.Forms.MessageBox.Show("Json Generate Failed");
                Application.Exit();
            }
        }

        private object RandomDay(Random rnd)
        {
            DateTime start = new DateTime(1910, 1, 1);
            int range = (DateTime.Today - start).Days;
            return start.AddDays(rnd.Next(range));
        }

        private string RandomStringForName(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                 .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private static int RandomNumber(Random rnd, int min, int max)
        {
            int number = rnd.Next(min, max);
            return number;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {

                rowCount = textBox1.Text;
                columnCount = textBox2.Text;

                var labelInfo = new System.Windows.Forms.Label();
                labelInfo.AutoSize = true;
                labelInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
                labelInfo.Location = new System.Drawing.Point(57, 150);
                labelInfo.Name = "labelInfo";
                labelInfo.Size = new System.Drawing.Size();
                labelInfo.TabIndex = 8;
                labelInfo.Text = " Please enter the column info.";

                this.Controls.Add(labelInfo);

                int ylock = 200;

                for (int i = 0; i < Convert.ToInt32(columnCount); i++)
                {
                    var label = new System.Windows.Forms.Label();
                    label.AutoSize = true;
                    label.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
                    label.Location = new System.Drawing.Point(57, ylock);
                    label.Name = "label" + (i + 4).ToString();
                    label.Size = new System.Drawing.Size(121, 21);
                    label.TabIndex = 1;
                    label.Text = "Columns" + (i + 1).ToString();

                    var comboBox1 = new System.Windows.Forms.ComboBox();
                    comboBox1.FormattingEnabled = true;
                    comboBox1.AutoSize = true;
                    comboBox1.Location = new System.Drawing.Point(375, ylock);
                    comboBox1.Name = "comboBox" + (i).ToString();
                    comboBox1.Size = new System.Drawing.Size(121, 21);
                    comboBox1.TabIndex = 6;
                    comboBox1.DisplayMember = "Value";
                    comboBox1.ValueMember = "Key";
                    comboBox1.DataSource = new BindingSource(GetDataSource(), null);
                    comboBox1.SelectedIndexChanged += new EventHandler(this.comboBox1_selectedIndexChange);

                    var columnNameDynamic = new System.Windows.Forms.TextBox();
                    columnNameDynamic.Location = new System.Drawing.Point(250, ylock);
                    columnNameDynamic.Name = "textboxDynamic" + i.ToString();
                    columnNameDynamic.Size = new Size(106, 20);
                    columnNameDynamic.TabIndex = 1;

                    var numberRangeDynamic = new System.Windows.Forms.TextBox();
                    numberRangeDynamic.Location = new System.Drawing.Point(500, ylock);
                    numberRangeDynamic.Name = "textboxDynamicZoneFirst" + i.ToString();
                    numberRangeDynamic.Size = new Size(80, 20);
                    numberRangeDynamic.TabIndex = 10;
                    numberRangeDynamic.Visible = false;

                    var numberRangeDynamicSec = new System.Windows.Forms.TextBox();
                    numberRangeDynamicSec.Location = new System.Drawing.Point(600, ylock);
                    numberRangeDynamicSec.Name = "textboxDynamicZoneSecond" + i.ToString();
                    numberRangeDynamicSec.Size = new Size(80, 20);
                    numberRangeDynamicSec.TabIndex = 11;
                    numberRangeDynamicSec.Visible = false;

                    ylock += 30;



                    this.Controls.Add(label);
                    this.Controls.Add(comboBox1);
                    this.Controls.Add(columnNameDynamic);
                    this.Controls.Add(numberRangeDynamic);
                    this.Controls.Add(numberRangeDynamicSec);

                }
                button1.Visible = false;
                button2.Visible = true;

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Json Generate Failed");
                Application.Exit();
            }


        }

        private void comboBox1_selectedIndexChange(object sender, EventArgs e)
        {
            for (int i = 0; i < Convert.ToInt32(columnCount); i++)
            {
                var combo = this.Controls.Find("comboBox" + i.ToString(), true);
                var minVal = this.Controls.Find("textboxDynamicZoneFirst" + i.ToString(), true);
                var maxVal = this.Controls.Find("textboxDynamicZoneSecond" + i.ToString(), true);

                minVal[0].Visible = combo[0].Text.ToLower() == "ınteger" ? true : false;
                maxVal[0].Visible = combo[0].Text.ToLower() == "ınteger" ? true : false;

            }
        }
        private Dictionary<int, string> GetDataSource()
        {
            Dictionary<int, string> comboSrc = new Dictionary<int, string>();
            comboSrc.Add(1, "String");
            comboSrc.Add(2, "Integer");
            comboSrc.Add(3, "Date");

            return comboSrc;
        }

        public static Type CompileResultType(List<Field> fields)
        {
            TypeBuilder tb = GetTypeBuilder();
            ConstructorBuilder constructor =
                tb.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName |
                                            MethodAttributes.RTSpecialName);

            foreach (var field in fields)
                CreateProperty(tb, field.FieldName, field.FieldType);

            Type objectType = tb.CreateType();
            return objectType;
        }

        private static TypeBuilder GetTypeBuilder()
        {
            var typeSignature = "MyDynamicType";
            var an = new AssemblyName(typeSignature);
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(an,
                AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
            TypeBuilder tb = moduleBuilder.DefineType(typeSignature
                , TypeAttributes.Public |
                  TypeAttributes.Class |
                  TypeAttributes.AutoClass |
                  TypeAttributes.AnsiClass |
                  TypeAttributes.BeforeFieldInit |
                  TypeAttributes.AutoLayout
                , null);
            return tb;
        }

        private static void CreateProperty(TypeBuilder tb, string propertyName, Type propertyType)
        {
            FieldBuilder fieldBuilder = tb.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);

            PropertyBuilder propertyBuilder = tb.DefineProperty(propertyName, System.Reflection.PropertyAttributes.HasDefault,
                propertyType, null);
            MethodBuilder getPropMthdBldr = tb.DefineMethod("get_" + propertyName,
                MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propertyType,
                Type.EmptyTypes);
            ILGenerator getIl = getPropMthdBldr.GetILGenerator();

            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, fieldBuilder);
            getIl.Emit(OpCodes.Ret);

            MethodBuilder setPropMthdBldr =
                tb.DefineMethod("set_" + propertyName,
                    MethodAttributes.Public |
                    MethodAttributes.SpecialName |
                    MethodAttributes.HideBySig,
                    null, new[] { propertyType });

            ILGenerator setIl = setPropMthdBldr.GetILGenerator();
            System.Reflection.Emit.Label modifyProperty = setIl.DefineLabel();
            System.Reflection.Emit.Label exitSet = setIl.DefineLabel();

            setIl.MarkLabel(modifyProperty);
            setIl.Emit(OpCodes.Ldarg_0);
            setIl.Emit(OpCodes.Ldarg_1);
            setIl.Emit(OpCodes.Stfld, fieldBuilder);

            setIl.Emit(OpCodes.Nop);
            setIl.MarkLabel(exitSet);
            setIl.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getPropMthdBldr);
            propertyBuilder.SetSetMethod(setPropMthdBldr);
        }
    }
}
