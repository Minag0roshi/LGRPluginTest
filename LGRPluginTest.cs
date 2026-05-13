using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tekla.Structures;
using Tekla.Structures.Internal;
using Tekla.Structures.Model;
using Tekla.Structures.Plugins;
using T3D = Tekla.Structures.Geometry3d;
using TSDT = Tekla.Structures.Datatype;
using TSM = Tekla.Structures.Model;
using TSMUI = Tekla.Structures.Model.UI;
using TSO = Tekla.Structures.Model.Operations;
using TSP = Tekla.Structures.Plugins;

namespace LGRPluginTest
{

    #region Data Passed From UI to Plugin

    // the "LengthFactor" is the AttributeName of the TextBox for example

    public class StructuresData
    {
        [StructuresField("LengthFactor")]
        public double LengthFactor;
        [StructuresField("Profile")]
        public string Profile;
    }

    #endregion


    #region Plugin Description
    // Description of this plugin.
    // First the plugin name
    // then the UI for the plugin
    // after that for translation purposes
    [TSP.Plugin("LGRPluginTest")]
    [TSP.PluginUserInterface("LGRPluginTest.MainForm")]
    [TSP.PluginDescription("enu", "LGRPluginTest")]
    [TSP.PluginCoordinateSystem(PluginBase.CoordinateSystemType.FROM_FIRST_AND_SECOND_POINT)]
    //This one would be where the insertion point of the plugin changed, the plugin would re-run
    [TSP.InputObjectDependency(InputObjectDependency.DEPENDENT)]
    // [TSP.InputObjectDependency(InputObjectDependency.NOT_DEPENDENT_MODIFIABLE)]

    #endregion


    #region Plugin Definition

    // Here is where we are actually defining the Tekla Structures plugin class
    // LGRPluginTest is the plugin name
    // PluginBase because is model, not drawing
    public class LGRPluginTest : TSP.PluginBase
    {

        #region Fields

        // Enables inserting of objects in a model.
        private readonly TSM.Model _model;
        // Enables retrieving of input values.
        private StructuresData Data { get; set; }

        private double _LengthFactor;
        private string _Profile;


        // private int NumberOfBeams = 5;


        #endregion

        public TSM.Model Model
        {
            get { return _model; }
        }

        #region Constructor

        public LGRPluginTest(StructuresData data)
        {
            // Link to model.
            _model = new TSM.Model();

            // Link to input values.
            Data = data;
        }

        #endregion


        #region Inputs method that gets user to pick points and parts used in the plugin


        // Specifies the user input needed for the plugin.
        // <returns>List of input definitions</returns>
        public override List<InputDefinition> DefineInput()
        {
            TSMUI.Picker BeamPicker = new TSMUI.Picker();
            List<InputDefinition> PointList = new List<InputDefinition>();

            T3D.Point Point1 = BeamPicker.PickPoint();
            T3D.Point Point2 = BeamPicker.PickPoint();

            InputDefinition Input1 = new InputDefinition(Point1);
            InputDefinition Input2 = new InputDefinition(Point2);

            //Add inputs to InputDefinition list.
            PointList.Add(Input1);
            PointList.Add(Input2);

            return PointList;
        }


        #endregion





        #region Create Beam

        // DON'T CALL COMMIT CHANGES. PLUGINS HANDLE THIS INTERNALLY
        // Model.CommitChanges();

        private void CreateBeam(T3D.Point Point1, T3D.Point Point2)
        {
            TSM.Beam MyBeam = new TSM.Beam(Point1, Point2);

            MyBeam.Profile.ProfileString =  _Profile;
            MyBeam.Finish = "HDG";
     

            // With this we help internal code to assign same ID to beam when plugin is modified.
            // To avoid some problems related to links with UDA values or booleans (cuts, fittings) for example.
            MyBeam.SetLabel("MyBeam01");

            MyBeam.Insert();
        }

        #endregion



        #region Get Values from dialog (example) --> GetValuesFromDialog()

        private void GetValuesFromDialog()
        {
            _LengthFactor = Data.LengthFactor;
            _Profile = Data.Profile;

            if (IsDefaultValue(_LengthFactor))
            {
                _LengthFactor = 2.0;
            }
            if (IsDefaultValue(_Profile))
            {
                _Profile = "HEA300";
            }
        }


        #endregion



        #region Run Method that executes the code. Main method of the plug-in.

        // This function is called upon execution of the plugin.
        // <param name="input">List of input definitions</param>
        // <returns></returns>
        public override bool Run(List<InputDefinition> Input)
        {

                // Write your code here.

                // Gets values from the StructuresData class and sets them to the fields in the plugin
                // If fields left empty on dialog, the hard coded defaults are set.

                // For example: GetValuesFromDialog();

                // Change the workplane further, or do something with the pickpoints if needed


                // Your Execution Code Goes Here!

                // A good practice is to have the class in a separate .cs to be reused on other plugin or app. Below is not the ideal...
                // CreateBuilding(PickedPointDistance);


            try
            {
                GetValuesFromDialog();

                T3D.Point Point1 = (T3D.Point)(Input[0]).GetInput();
                T3D.Point Point2 = (T3D.Point)(Input[1]).GetInput();

                T3D.Point LengthVector = new T3D.Point(
                    Point2.X - Point1.X,
                    Point2.Y - Point1.Y,
                    Point2.Z - Point1.Z
                );

                if (_LengthFactor > 0)
                {
                    Point2.X = _LengthFactor * LengthVector.X + Point1.X;
                    Point2.Y = _LengthFactor * LengthVector.Y + Point1.Y;
                    Point2.Z = _LengthFactor * LengthVector.Z + Point1.Z;
                }

                CreateBeam(Point1, Point2);
            }
            catch (Exception Ex)
            {
                Console.WriteLine("Exception: " + Ex.ToString());
            }

            return true;
        }


        #endregion


    }

    #endregion

}