using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Timers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;


namespace Assets.Scripts
{
    public class PluxUnityInterface : MonoBehaviour
    {
        // Declaration of public variables.
        // [Graphical User Interface Objects]
        public Dropdown DeviceDropdown;
        public Button StartButton;
        public Button StopButton;
        public Button ScanButton;
        public Button AboutButton;
        public Button ConnectButton;
        public Button ChangeChannel;
        public InputField SamplingRateInput;
        public InputField ResolutionInput;
        public Dropdown ResolutionDropdown;
        public Toggle CH1Toggle;
        public Toggle CH2Toggle;
        public Toggle CH3Toggle;
        public Toggle CH4Toggle;
        public Toggle CH5Toggle;
        public Toggle CH6Toggle;
        public Toggle CH7Toggle;
        public Toggle CH8Toggle;
        public Toggle BTHToggle;
        public Toggle BLEToggle;
        public GameObject SamplingRateInfoPanel;
        public GameObject SelectedChannelPanel;
        public GameObject ChannelSelectioInfoPanel;
        public GameObject ConnectInfoPanel;
        public GameObject BluetoothInfoPanel;
        public GameObject BLESamplingRateInfoPanel;
        public GameObject BatteryIconUnknown;
        public GameObject BatteryIcon0;
        public GameObject BatteryIcon10;
        public GameObject BatteryIcon50;
        public GameObject BatteryIcon100;
        public GameObject PlotIcon;
        public GameObject AcquiringIcon;
        public GameObject GreenFlag;
        public GameObject RedFlag;
        public GameObject TransparencyLevel;
        public Text ConnectText;
        public Text BatteryLevel;
        public Text CurrentChannel;
        public RectTransform GraphContainer;
        public RectTransform GraphContainer2;
        [SerializeField] public Sprite DotSprite;
        public WindowGraph GraphZone;
        public WindowGraph GraphZone2;

        // [Delegate References]
        // Delegates (needed for callback purposes).
        public delegate bool FPtr(int nSeq, IntPtr dataIn, int dataInSize);

        // [Generic Variables]
        [SerializeField] public PluxDeviceManager PluxDevManager;
        public List<List<int>> MultiThreadList = null;
        public List<int> MultiThreadSubList = null;

        public List<int> ActiveChannels;
        public List<string> ListDevices;
        public int tempInt;
        public int LastLenMultiThreadString = 0;
        public int GraphWindSize = -1;
        public bool FirstPlot = true;
        public bool FirstPlot2 = true;
        public List<string> ResolutionDropDownOptions = new List<string>() { "8", "16" };
        public int VisualizationChannel = -1;
        public int SamplingRate;
        public int WindowInMemorySize;
        public bool UpdatePlotFlag = false;
        public string SelectedDevice = "";


        //////////////////////////////// Edit by Lillian Fan//////////////////////////////
        // [Addtional Variable]
        // Different sensor's record data in string
        string EMGData = "";
        string ECGData = "";
        string ECGHR = "";
        string EDAData = "";
        string FolderPath = "";

        // Different sensor's record data in list
        List<double> EMGDataList = new List<double>() { };
        List<double> RMSDataList = new List<double>() { };
        int RMSSampleRate = 100;

        // Boolean for if certain datain been update in cetain time interval or not
        bool EMGUpdate = false;
        bool ECGUpdate = false;
        bool EDAUpdate = false;

        // Base line for EMG and EDA
        public double thoresholdEMG = 0.1f;
        public float thoresholdEDA = 0.5f;

        // Data use to compare EDA each 5s
        int sampleingCount = 1;
        List<double> EDAPre = new List<double>() { };
        List<double> EDANow = new List<double>() { };


        // Feedback
        public Image flashImage;
        private bool isFlashing = false;
        public float flashSpeed;
        public Color flashColor;

        // Calculate heart rate
        double[] HBtens = new double[] { };
        int HBCount = 1;
        public Text HRText;


        //Create a timer that controls the update of real-time plot.
        System.Timers.Timer waitForPlotTimer = new System.Timers.Timer();
        //////////////////////////////////////////////////////////////////////////////////


        ///////////////////////////////////Edited by Kody Wood/////////////////////////////////////////////
        //Variables that will hold the data from the channel we are looking for.
        //Second graph variables
        public bool UpdatePlotFlag2 = false;
        [Tooltip("How long to record data when activated")]
        public float recordTime = 1000.0f;
        [Tooltip("How long the data has been recorded")]
        public float recordTimer;

        // Variables to send variables to difficulty script
        List<double> EMGDataCalc = new List<double>() { }; //For completion
        List<double> RMSDataCalc = new List<double>() { }; //For completion
        List<double> EDADataCalc = new List<double>() { };
        List<double> ECGDataCalc = new List<double>() { };
        List<float> ECGDataHRCalc = new List<float>() { }; //This is to calculate the HR, we need to use float to access the Mathf.Max function
        int heartRate = 0;
        public DifficultyCalculator eventManager;
        /////////////////////////////////////////////////////////////////////////////////////////

        // Awake is called when the script instance is being loaded.
        void Awake()
        {
            // Find references to graphical objects.
            GraphContainer = transform.Find("WindowGraph/EDA GraphContainer").GetComponent<RectTransform>();  // User interface zone where the acquired data will be plotted using the "WindowGraph.cs" script.
            GraphContainer2 = transform.Find("WindowGraph/ECG GraphContainer").GetComponent<RectTransform>();  // User interface zone where the acquired data will be plotted using the "WindowGraph.cs" script.
        }
        // Start is called before the first frame update
        void Start()
        {
            // Welcome Message, showing that the communication between C++ dll and Unity was established correctly.
            Debug.Log("Connection between C++ Interface and Unity established with success !\n");
            PluxDevManager = new PluxDeviceManager();
            int welcomeNumber = PluxDevManager.WelcomeFunctionUnity();
            Debug.Log("Welcome Number: " + welcomeNumber);

            // Initialization of Variables.      
            MultiThreadList = new List<List<int>>();
            ActiveChannels = new List<int>();

            // Initialization of graphical zone.
            WindowGraph.IGraphVisual graphVisual = new WindowGraph.LineGraphVisual(GraphContainer, DotSprite, new Color(0, 158, 227, 0), new Color(0, 158, 227));
            GraphContainer = graphVisual.GetGraphContainer();
            GraphZone = new WindowGraph(GraphContainer, graphVisual);
            GraphZone.ShowGraph(new List<int>() { 0 }, graphVisual, -1, (int _i) => "" + (_i), (float _f) => Mathf.RoundToInt(_f) + "k");

            //////////////////////////////// Edit by Lillian Fan//////////////////////////////
            // Initial second graph
            WindowGraph.IGraphVisual graphVisual2 = new WindowGraph.LineGraphVisual(GraphContainer2, DotSprite, new Color(0, 158, 227, 0), new Color(0, 158, 227));
            GraphContainer2 = graphVisual2.GetGraphContainer();
            GraphZone2 = new WindowGraph(GraphContainer2, graphVisual2);
            GraphZone2.ShowGraph(new List<int>() { 0 }, graphVisual2, -1, (int _i) => "" + (_i), (float _f) => Mathf.RoundToInt(_f) + "k");
            //////////////////////////////////////////////////////////////////////////////////


            // Create a timer that controls the update of real-time plot.
            //System.Timers.Timer waitForPlotTimer = new System.Timers.Timer();
            waitForPlotTimer.Elapsed += new ElapsedEventHandler(OnWaitingTimeEnds);
            waitForPlotTimer.Interval = 1000; // 1 second.
            waitForPlotTimer.Enabled = false;
            waitForPlotTimer.AutoReset = true;

;
;
        }
        // Update function, being constantly invoked by Unity.
        void Update()
        {
            ////////////////////////////////////////////////// Edit by Lillian Fan////////////////////////////////////////////////////
            
            
            // Update flash image when EMG or EDA change higher than the baseline
            if (isFlashing)
            {
                flashImage.color = flashColor;
                isFlashing = false;
            }
            else
            {
                flashImage.color = Color.Lerp(flashImage.color, Color.clear, flashSpeed * Time.deltaTime);
            }

            // Press Different Key to control the connection between Bitalino and Unity
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                // Searching device
                ExecuteEvents.Execute(ScanButton.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                // Connect to device
                ExecuteEvents.Execute(ConnectButton.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                // Start recording
                ExecuteEvents.Execute(StartButton.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                // Stop recording
                ExecuteEvents.Execute(StopButton.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
            }
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            try
            {

                ////////////////////////////////////////////////// Edit by Lillian Fan///////////////////////////////////////////////////////////////////////
                ////////////////////////////////////////////////Record EMG,ECG and EDA Data//////////////////////////////////////////////////////////////////

                //int[] packageOfDataEMG = PluxDevManager.GetPackageOfData(1, ActiveChannels, EMGUpdate);
                //if (EMGUpdate == true && packageOfDataEMG != null && packageOfDataEMG.Length != 0)
                //{
                //    RecordEMG(packageOfDataEMG);
                //}


                if ((eventManager.GetComponent<EventManager>().recordBaseline || eventManager.GetComponent<EventManager>().recordMetrics) && recordTimer == 0.0f)
                {
                    // Start recording
                    waitForPlotTimer.Enabled = true;
                }

                if (waitForPlotTimer.Enabled)
                { 
                    //ECG data record and reaction
                    int[] packageOfDataECG = PluxDevManager.GetPackageOfData(2, ActiveChannels, ECGUpdate);
                    if (ECGUpdate == true && packageOfDataECG != null && packageOfDataECG.Length != 0)
                    {
                        RecordECG(packageOfDataECG);
                    }
                    //EDA data record and reaction
                    int[] packageOfDataEDA = PluxDevManager.GetPackageOfData(3, ActiveChannels, EDAUpdate);
                    if (EDAUpdate == true && packageOfDataEDA != null && packageOfDataEDA.Length != 0)
                    {
                        RecordEDA(packageOfDataEDA);
                    }

                    recordTimer += Time.deltaTime;
                    if (recordTimer >= recordTime)
                    {
                        
                        // Update EMG baseline after first 30s record and reset all variable
                        // Save baseline Stats and reset all variables for other measurements
                        if (eventManager.GetComponent<EventManager>().isFirstRecording)
                        {
                            //EMG baseline recording for my thesis we do not need
                            //thoresholdEMG = RMSDataList.Average() + 0.01;
                            //RMSDataList.Clear();
                            Debug.Log("Baseline measurements");
                            //Calculate the heart rate once we have all the data
                            CalculateHeartRate(ECGDataHRCalc);
                            //Send baseline measurements to store
                            eventManager.baseLineEDA = new List<double>(EDADataCalc);
                            eventManager.baseLineECG = new List<double>(ECGDataCalc);
                            eventManager.baseHeartRate = heartRate;
                            eventManager.CalculateBaseDataAverage();

                            //Reset
                            EDADataCalc.Clear();
                            ECGDataCalc.Clear();
                            ECGDataHRCalc.Clear();

                            eventManager.GetComponent<EventManager>().isFirstRecording = false;
                            eventManager.GetComponent<EventManager>().recordBaseline = false;
                            //eventManager.GetComponent<EventManager>().DoneBaseline = true;
                        }
                        else
                        {
                            Debug.Log("Adaptive measurements");
                            waitForPlotTimer.Enabled = false;
                            //Calculate the heart rate once we have all the data
                            CalculateHeartRate(ECGDataHRCalc);
                            //Send and Reset Variables
                            //Send
                            eventManager.currentEDA = new List<double>(EDADataCalc);
                            eventManager.currentECG = new List<double>(ECGDataCalc);
                            eventManager.heartRate = heartRate;
                            eventManager.CalculateDataAverage();
                            
                            //Reset
                            EDADataCalc.Clear();
                            ECGDataCalc.Clear();
                            ECGDataHRCalc.Clear();
                            eventManager.GetComponent<EventManager>().recordMetrics = false;
                        }

                        // Stop recording
                        waitForPlotTimer.Enabled = false;
                        recordTimer = 0.0f;
                    }
                    
                }
                else
                {
                    UpdatePlotFlag = true;
                    UpdatePlotFlag2 = true;
                }

                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                
                // Get packages of data that will be shown on the graphic
                //int[] packageOfData = PluxDevManager.GetPackageOfData(2, ActiveChannels, UpdatePlotFlag);
                ////int[] packageOfData = PluxDevManager.GetPackageOfData(VisualizationChannel, ActiveChannels, UpdatePlotFlag); //This will be for the graphic 
                //
                //// Check if there it was communicated an event/error code.
                //if (packageOfData != null)
                //{
                //    if (packageOfData.Length != 0)
                //    {
                //        // Creation of the first graphical representation of the results.
                //        if (MultiThreadList[2].Count >= 0)
                //        {
                //            if (FirstPlot == true)
                //            {
                //                // Update flag (after this step we won't enter again on this statement).
                //                FirstPlot = false;
                //
                //                // Plot first set of data.
                //                // Subsampling if sampling rate is bigger than 100 Hz.
                //                List<int> subSamplingList = GetSubSampleList(new int[GraphWindSize], SamplingRate, GraphWindSize);
                //                GraphZone.ShowGraph(subSamplingList, null, -1, (int _i) => "-" + (GraphWindSize - _i),
                //                    (float _f) => Mathf.RoundToInt(_f / 1000) + "k");
                //            }
                //            // Update plot.
                //            else if (FirstPlot == false)
                //            {
                //                // This if clause sensures that the real-time plot will only be updated every 1 second (Memory Restrictions).
                //                if (UpdatePlotFlag == true && packageOfData != null)
                //                {
                //                    
                //                    MultiThreadSubList = GetSubSampleList(packageOfData, SamplingRate, GraphWindSize);
                //                    GraphZone.UpdateValue(MultiThreadSubList);
                //
                //                    // Reboot flag.
                //                    UpdatePlotFlag = false;
                //                }
                //            }
                //        }
                //    }
                //}
                //
                //
                /////////////////////////////////////EDA Graph//////////////////////////////////////////////////
                //int[] packageOfData2 = PluxDevManager.GetPackageOfData(3, ActiveChannels, UpdatePlotFlag2); //This will be for the graphic only
                //if (packageOfData2 != null)
                //{
                //    if (packageOfData2.Length != 0)
                //    {
                //        // Creation of the first graphical representation of the results.
                //        if (MultiThreadList[3].Count >= 0)
                //        {
                //            if (FirstPlot2 == true)
                //            {
                //                // Update flag (after this step we won't enter again on this statement).
                //                FirstPlot2 = false;
                //
                //                // Plot first set of data.
                //                // Subsampling if sampling rate is bigger than 100 Hz.
                //                List<int> subSamplingList = GetSubSampleList(new int[GraphWindSize], SamplingRate, GraphWindSize);
                //                GraphZone2.ShowGraph(subSamplingList, null, -1, (int _i) => "-" + (GraphWindSize - _i),
                //                    (float _f) => Mathf.RoundToInt(_f / 1000) + "k");
                //            }
                //            // Update plot.
                //            else if (FirstPlot2 == false)
                //            {
                //                // This if clause sensures that the real-time plot will only be updated every 1 second (Memory Restrictions).
                //                if (UpdatePlotFlag2 == true && packageOfData2 != null)
                //                {
                //                    //MultiThreadSubListPerChannel2.Add(GetSubSampleList(packageOfData2, SamplingRate, GraphWindSize));
                //                    MultiThreadSubList = GetSubSampleList(packageOfData2, SamplingRate, GraphWindSize);
                //                    GraphZone2.UpdateValue(MultiThreadSubList);
                //
                //                    // Reboot flag.
                //                    UpdatePlotFlag2 = false;
                //                }
                //            }
                //        }
                //    }
                //}
            }
            catch (ArgumentOutOfRangeException exception)
            {
                Debug.Log("Exception in the Update method: " + exception.StackTrace);
                Console.WriteLine("Current Thread: " + Thread.CurrentThread.Name);
            }
            catch (ExternalException exc)
            {
                Debug.Log("ExternalException in the Update() callback:\n" + exc.Message + "\n" + exc.StackTrace);

                // Stop Acquisition in a secure way.
                StopButtonFunction(-1);
            }
            catch (Exception exc)
            {
                Debug.Log("Unidentified Exception inside Update() callback:\n" + exc.Message + "\n" + exc.StackTrace);
            }
        }

        // Method invoked when the application was closed.
        void OnApplicationQuit()
        {
            //////////////////////////////// Edit by Lillian Fan//////////////////////////////
            // Out put the EMG and EDA data to the file
            OutPutEMG();
            OutPutEDA();
            OutPutECG();
            // Clean up EMG and EDA's data
            EMGData = "";
            EDAData = "";
            ECGData = "";
            //////////////////////////////////////////////////////////////////////////////////
            ///
            // Disconnect from device.
            PluxDevManager.DisconnectPluxDev();
            Debug.Log("Application ending after " + Time.time + " seconds");
        }

        // ===========================================================================================================================================
        // ================================ Functions invoked when after a specific interaction with GUI elements ====================================
        // ===========================================================================================================================================
        // Function invoked during the onClick event of "ScanButton".
        public void ScanButtonFunction()
        {
            try
            {
                // List of available Devices.
                List<string> listOfDomains = new List<string>();
                if (BTHToggle.isOn)
                {
                    listOfDomains.Add("BTH");
                }
                if (BLEToggle.isOn)
                {
                    listOfDomains.Add("BLE");
                }

                this.ListDevices = PluxDevManager.GetDetectableDevicesUnity(listOfDomains);
                // Info message for development purposes.
                Console.WriteLine("Number of Detected Devices: " + this.ListDevices.Count);
                for (int i = 0; i < this.ListDevices.Count; i++)
                {
                    Console.WriteLine("Device--> " + this.ListDevices[i]);
                }

                // Enable Dropdown if the list of devices is not empty.
                if (this.ListDevices.Count != 0)
                {
                    // Add the new options to the drop-down box included in our GUI.
                    //Create a List of new Dropdown options
                    List<string> dropDevices = new List<string>();

                    // Convert array to list format.
                    dropDevices.AddRange(this.ListDevices);

                    // A check into the list of devices.
                    dropDevices = dropDevices.GetRange(0, dropDevices.Count);
                    for (int i = dropDevices.Count - 1; i >= 0; i--)
                    {
                        // Accept only strings containing "BTH" or "BLE" substrings "flagging" a PLUX Bluetooth device.
                        if (!dropDevices[i].Contains("BTH") && !dropDevices[i].Contains("BLE"))
                        {
                            dropDevices.RemoveAt(i);
                        }
                    }

                    // Raise an exception if none device was detected.
                    if (dropDevices.Count == 0)
                    {
                        throw new ArgumentException();
                    }

                    //Clear the old options of the Dropdown menu
                    DeviceDropdown.ClearOptions();

                    //Add the options created in the List above
                    DeviceDropdown.AddOptions(dropDevices);

                    // Enable drop-down and Connect button if a PLUX Device was detected .
                    DeviceDropdown.interactable = true;
                    ConnectButton.interactable = true;

                    // Hide info message.
                    ConnectInfoPanel.SetActive(false);

                }
                
            }
            catch (Exception e)
            {
                // Show info message.
                BluetoothInfoPanel.SetActive(true);

                // Hide object after 5 seconds.
                StartCoroutine(RemoveAfterSeconds(5, BluetoothInfoPanel));

                // Disable Drop-down.
                DeviceDropdown.interactable = false;
            }
        }

        // Function invoked during the onClick event of "ConnectButton".
        public void ConnectButtonFunction(bool typeOfStop)
        {
            try
            {
                
                // Change the color and text of "Connect" button.
                if (ConnectText.text == "Connect")
                {
                    // Get the selected device.
                    this.SelectedDevice = this.ListDevices[DeviceDropdown.value];

                    // Connection with the device.
                    Debug.Log("Trying to establish a connection with device " + this.SelectedDevice);
                    Console.WriteLine("Selected Device: " + this.SelectedDevice);
                    PluxDevManager.PluxDev(this.SelectedDevice);
                    Debug.Log("Connection with device " + this.SelectedDevice + " established with success!");

                    ConnectText.text = "Disconnect";
                    GreenFlag.SetActive(true);
                    RedFlag.SetActive(false);

                    // Enable "Device Configuration" panel options.
                    SamplingRateInput.interactable = true;
                    ResolutionInput.interactable = true;
                    ResolutionDropdown.interactable = true;

                    // Enable channel selection buttons accordingly to the type of device.
                    string devType = PluxDevManager.GetDeviceTypeUnity();
                    if (devType == "MuscleBAN BE Plux")
                    {
                        CH1Toggle.interactable = true;

                        //Clear the old options of the Dropdown menu
                        ResolutionDropdown.ClearOptions();

                        //Add the options created in the List above
                        ResolutionDropdown.AddOptions(new List<string>() { "8", "16" });
                    }
                    else if (devType == "BITalino")
                    {
                        CH1Toggle.interactable = true;
                        CH2Toggle.interactable = true;
                        CH3Toggle.interactable = true;
                        CH4Toggle.interactable = true;
                        CH5Toggle.interactable = true;
                        CH6Toggle.interactable = true;

                        //Clear the old options of the Dropdown menu
                        ResolutionDropdown.ClearOptions();

                        //Add the options created in the List above
                        ResolutionDropdown.AddOptions(new List<string>() { "10" });
                    }
                    else if (devType == "biosignalsplux" || devType == "BioPlux")
                    {
                        CH1Toggle.interactable = true;
                        CH2Toggle.interactable = true;
                        CH3Toggle.interactable = true;
                        CH4Toggle.interactable = true;
                        CH5Toggle.interactable = true;
                        CH6Toggle.interactable = true;
                        CH7Toggle.interactable = true;
                        CH8Toggle.interactable = true;

                        //Clear the old options of the Dropdown menu
                        ResolutionDropdown.ClearOptions();

                        //Add the options created in the List above
                        if (devType == "biosignalsplux")
                        {
                            ResolutionDropdown.AddOptions(new List<string>() { "8", "16" });
                            ResolutionDropDownOptions = new List<string>() { "8", "16" };
                        }
                        else
                        {
                            ResolutionDropdown.AddOptions(new List<string>() { "8", "12" });
                            ResolutionDropDownOptions = new List<string>() { "8", "12" };
                        }
                    }
                    else if (devType == "OpenBANPlux")
                    {
                        CH1Toggle.interactable = true;
                        CH2Toggle.interactable = true;

                        //Clear the old options of the Dropdown menu
                        ResolutionDropdown.ClearOptions();

                        //Add the options created in the List above
                        ResolutionDropdown.AddOptions(new List<string>() { "8", "16" });
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }

                    // Enable Start and Device Configuration buttons.
                    StartButton.interactable = true;

                    // Disable Connect Button.
                    //ConnectButton.interactable = false;

                    // Hide show Info message if it is active.
                    ConnectInfoPanel.SetActive(false);

                    // Update Battery Level.
                    int batteryLevel = -1;
                    if (devType != "BioPlux")
                    {
                        batteryLevel = PluxDevManager.GetBatteryUnity();
                    }

                    // Battery icon accordingly to the battery level.
                    List<GameObject> ListBatteryIcons = new List<GameObject>() { BatteryIcon0, BatteryIcon10, BatteryIcon50, BatteryIcon100, BatteryIconUnknown };
                    GameObject currImage;
                    if (batteryLevel > 50)
                    {
                        BatteryIcon100.SetActive(true);
                        currImage = BatteryIcon100;
                    }
                    else if (batteryLevel <= 50 && batteryLevel > 10)
                    {
                        BatteryIcon50.SetActive(true);
                        currImage = BatteryIcon50;
                    }
                    else if (batteryLevel <= 10 && batteryLevel > 1)
                    {
                        BatteryIcon10.SetActive(true);
                        currImage = BatteryIcon10;
                    }
                    else if (batteryLevel == 0)
                    {
                        BatteryIcon0.SetActive(true);
                        currImage = BatteryIcon0;
                    }
                    else
                    {
                        BatteryIconUnknown.SetActive(true);
                        currImage = BatteryIconUnknown;
                    }

                    // Disable the remaining images.
                    foreach (var batImg in ListBatteryIcons)
                    {
                        if (batImg != currImage)
                        {
                            batImg.SetActive(false);
                        }
                    }

                    // Show the quantitative battery value.
                    if (batteryLevel != -1)
                    {
                        BatteryLevel.text = batteryLevel.ToString() + "%";
                    }
                    else
                    {
                        BatteryLevel.text = "N.A.";
                    }

                    //Bool to check if it is connect, this will be checked by the event manager to change task from Freeroam to baseline recording
                    eventManager.GetComponent<EventManager>().isConnected = true;
                }
                else if (ConnectText.text == "Disconnect")
                {

                    try
                    {
                        // Disconnect device.
                        PluxDevManager.DisconnectPluxDev();
                    }
                    catch (Exception exception)
                    {
                        Debug.Log("Trying to disconnect from an unconnected device...");
                    }

                    ConnectText.text = "Connect";
                    GreenFlag.SetActive(false);
                    RedFlag.SetActive(true);

                    // Disable "Device Configuration" panel options.
                    SamplingRateInput.interactable = false;
                    SamplingRateInput.text = "100";
                    ResolutionInput.interactable = false;
                    ResolutionDropdown.interactable = false;

                    // Disable channel selection buttons.
                    CH1Toggle.interactable = false;
                    CH2Toggle.interactable = false;
                    CH3Toggle.interactable = false;
                    CH4Toggle.interactable = false;
                    CH5Toggle.interactable = false;
                    CH6Toggle.interactable = false;
                    CH7Toggle.interactable = false;
                    CH8Toggle.interactable = false;

                    // Disable Start and Device Configuration buttons.
                    StartButton.interactable = false;

                    // Disable the battery icons.
                    List<GameObject> ListBatteryIcons = new List<GameObject>() { BatteryIcon0, BatteryIcon10, BatteryIcon50, BatteryIcon100 };
                    foreach (var batImg in ListBatteryIcons)
                    {
                        batImg.SetActive(false);
                    }
                    BatteryIconUnknown.SetActive(true);

                    // Show the quantitative battery value.
                    BatteryLevel.text = "";

                    // Disable Drop-down options.
                    DeviceDropdown.ClearOptions();

                    //Add the options created in the List above
                    DeviceDropdown.AddOptions(new List<string>() { "Select Device" });

                    // Disable drop-down and Connect button if a PLUX Device was disconnected.
                    DeviceDropdown.interactable = false;
                    ConnectButton.interactable = false;

                    // Show PlotIcon.
                    PlotIcon.SetActive(true);
                    TransparencyLevel.SetActive(true);

                    // Reboot of global variables.
                    RebootVariables();

                    eventManager.GetComponent<EventManager>().isConnected = false;
                }
            }
            catch (Exception e)
            {
                // Print information about the exception.
                Debug.Log(e);

                // Show info message.
                ConnectInfoPanel.SetActive(true);

                // Hide object after 5 seconds.
                StartCoroutine(RemoveAfterSeconds(5, ConnectInfoPanel));
            }
        }

        // Function invoked during the onClick event of "StartButton".
        public void StartButtonFunction()
        {
            try
            {
                // Get Device Configuration input values.
                SamplingRate = Int32.Parse(SamplingRateInput.text);
                int resolution = Int32.Parse(ResolutionDropDownOptions[ResolutionDropdown.value]);

                // Update graphical window size variable (the plotting zone should contain 10 seconds of data).
                GraphWindSize = SamplingRate * 10;
                WindowInMemorySize = Convert.ToInt32(1.1 * GraphWindSize);

                // Number of Active Channels.
                int nbrChannels = 0;
                Toggle[] toggleArray = new Toggle[]
                    {CH1Toggle, CH2Toggle, CH3Toggle, CH4Toggle, CH5Toggle, CH6Toggle, CH7Toggle, CH8Toggle};
                MultiThreadList.Add(new List<int>(Enumerable.Repeat(0, GraphWindSize).ToList()));
                for (int i = 0; i < toggleArray.Length; i++)
                {
                    if (toggleArray[i].isOn == true)
                    {
                        // Preparation of a string that will be communicated to our .dll
                        // This string will be formed by "1" or "0" characters, identifying sequentially which channels are active or not.
                        ActiveChannels.Add(i + 1);

                        // Definition of the first active channel.
                        if (VisualizationChannel == -1)
                        {
                            VisualizationChannel = i + 1;

                            // Update the label with the Current Channel Number.
                            CurrentChannel.text = "CH" + VisualizationChannel;
                        }

                        nbrChannels++;
                    }

                    // Dictionary that stores all the data received from .dll API.
                    MultiThreadList.Add(new List<int>(Enumerable.Repeat(0, GraphWindSize).ToList()));
                }

                // Check if at least one channel is active.
                if (ActiveChannels.Count != 0)
                {

                    /////////////////////////////////////////// Edited by Kody Wood ////////////////////////////////////
                    ///                   // Update the label with the Current Channel Number.
                    CurrentChannel.text = "EMG";

                    // Create a folder for data
                    string t = "" + System.DateTime.Now;
                    t = t.Replace("/", ",").Replace(" ", ",").Replace(":", ",");
                    string path = Application.dataPath + "/BitalinoData" + "/" + t;
                    FolderPath = path;
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    //////////////////////////////////////////////////////////////////////////////////////////////////

                    // Start of Acquisition.
                    //Thread.CurrentThread.Name = "MAIN_THREAD";
                    if (PluxDevManager.GetDeviceTypeUnity() != "MuscleBAN BE Plux")
                    {
                        PluxDevManager.StartAcquisitionUnity(SamplingRate, ActiveChannels, resolution);
                    }
                    else
                    {
                        // Definition of the frequency divisor (subsampling ratio).
                        int freqDivisor = 10;
                        PluxDevManager.StartAcquisitionMuscleBanUnity(SamplingRate, ActiveChannels, resolution,
                            freqDivisor);
                    }

                    // Enable StopButton.
                    StopButton.interactable = true;

                    // Disable ConnectButton.
                    ConnectButton.interactable = false;

                    // Disable Start Button.
                    StartButton.interactable = false;

                    // Hide PlotIcon and show AcquiringIcon.
                    PlotIcon.SetActive(false);
                    TransparencyLevel.SetActive(false);
                    //AcquiringIcon.SetActive(true);

                    // Hide panel with the "Change Channel" button.
                    SelectedChannelPanel.SetActive(true);
                    if (ActiveChannels.Count == 1)
                    {
                        ChangeChannel.interactable = false;
                    }
                    else
                    {
                        ChangeChannel.interactable = true;
                    }

                    eventManager.GetComponent<EventManager>().isStarted = true;
                    // Disable About Button to avoid entering a new scene during the acquisition.
                    AboutButton.interactable = false;
                }
                else
                {
                    // Show Info Message.
                    ChannelSelectioInfoPanel.SetActive(true);

                    // Hide object after 5 seconds.
                    StartCoroutine(RemoveAfterSeconds(5, ChannelSelectioInfoPanel));
                }
            }
            catch (Exception exc)
            {
                // Exception info.
                Debug.Log("Exception: " + exc.Message + "\n" + exc.StackTrace);

                // Show info message.
                ConnectInfoPanel.SetActive(true);

                // Hide object after 5 seconds.
                StartCoroutine(RemoveAfterSeconds(5, ConnectInfoPanel));

                // Reboot interface.
                ConnectButtonFunction(true);
            }

        }

        // Function invoked during the onClick event of "StopButton".
        public void StopButtonFunction(int forceStop = 0)
        {
            // Invoke stop function from PluxDeviceManager.
            bool typeOfStop;

            // Check how many samples were communicated by the device.
            typeOfStop = PluxDevManager.StopAcquisitionUnity(forceStop);

            // Enable About Button.
            AboutButton.interactable = true;

            // Enable ConnectButton.
            if (StopButton.interactable == true)
            {
                ConnectButton.interactable = true;
            }

            // Enable ScanButton.
            ScanButton.interactable = true;

            // Disable StopButton.
            StopButton.interactable = false;

            // Disconnect device if a forced stop occurred.
            if (ConnectText.text == "Disconnect")
            {
                ConnectButtonFunction(typeOfStop);
            }

            // Show a warning message if something wrong happened.
            if (typeOfStop == true || forceStop == -1)
            {
                // Show info message.
                ConnectInfoPanel.SetActive(true);

                // Present a message stating the communication error and hide it after 5 seconds.
                StartCoroutine(RemoveAfterSeconds(5, ConnectInfoPanel));
            }

            // Hide info message.
            BLESamplingRateInfoPanel.SetActive(false);
        }

        // Function invoked during the onClick event of the About Button.
        public void AboutButtonFunction()
        {
            // Load About Scene.
            SceneManager.LoadScene("AboutWindow");

            // Reboot variables.
            if (ConnectText.text == "Disconnect")
            {
                ConnectButton.onClick.Invoke();
            }
            RebootVariables();
        }

        // Function invoked during the onValueChanged event of the Sampling Rate Input.
        public void SamplingRateOnChangeFunction()
        {
            // Check if "-" symbol is being introduced.
            if (SamplingRateInput.text != "")
            {
                if (SamplingRateInput.text.Contains("-") || SamplingRateInput.text.Length > 4 || Int32.Parse(SamplingRateInput.text) > 4000)
                {
                    // Show info message.
                    SamplingRateInfoPanel.SetActive(true);

                    // Place the standard Sampling rate.
                    SamplingRateInput.text = "1000";

                    // Hide object after 5 seconds.
                    StartCoroutine(RemoveAfterSeconds(5, SamplingRateInfoPanel));
                }
                // Check if we are dealing with BLE device. If so, when more than one channel is active the maximum sampling rate will be 100 Hz.
                else if (Int32.Parse(SamplingRateInput.text) > 100 && this.SelectedDevice.Contains("BLE") && GetNbrActiveToggle() > 1)
                {
                    // Force sampling rate to acquire the maximum value.
                    SamplingRate = 100;
                    SamplingRateInput.text = "100";

                    // Present info message.
                    BLESamplingRateInfoPanel.SetActive(true);

                    // Hide object after 5 seconds.
                    StartCoroutine(RemoveAfterSeconds(5, BLESamplingRateInfoPanel));
                }
            }
        }

        // Function invoked during the onValueChanged event of the Toggle Button Inputs.
        public void ToogleButtonOnChangeFunction()
        {
            if (Int32.Parse(SamplingRateInput.text) > 100 && this.SelectedDevice.Contains("BLE") && GetNbrActiveToggle() > 1)
            {
                // Force sampling rate to acquire the maximum value.
                SamplingRate = 100;
                SamplingRateInput.text = "100";

                // Present info message.
                BLESamplingRateInfoPanel.SetActive(true);

                // Hide object after 5 seconds.
                StartCoroutine(RemoveAfterSeconds(5, BLESamplingRateInfoPanel));
            }
        }

        // Function invoked during the onValueChanged event of the Bluetooth Toggle Button Inputs.
        public void BTToogleButtonOnChangeFunction(int btNbr)
        {
            Toggle[] toggleArray = new[] { BTHToggle, BLEToggle };
            if (!BTHToggle.isOn && !BLEToggle.isOn)
            {
                // Ignore the change command and keep the button active.
                toggleArray[btNbr].isOn = !toggleArray[btNbr].isOn;
            }
        }

        // Function invoked during the onValueChanged event of the Sampling Rate Input.
        public void SamplingRateOnEndEditFunction()
        {
            if (SamplingRateInput.text == "")
            {
                SamplingRateInput.text = "1000";
            }
        }

        // Function invoked during the onClick event of the "Change Channel" button.
        public void ChangeChannelClickFunction()
        {
            int indexOfVisualizationChn = ActiveChannels.IndexOf(VisualizationChannel);
            if (indexOfVisualizationChn == ActiveChannels.Count - 1)
            {
                VisualizationChannel = ActiveChannels[0];
            }
            else
            {
                VisualizationChannel = ActiveChannels[indexOfVisualizationChn + 1];
            }

            // Update the label with the Current Channel Number.
            if (VisualizationChannel == 1)
                CurrentChannel.text = "EMG";
            else if (VisualizationChannel == 2)
                CurrentChannel.text = "ECG";
            else if (VisualizationChannel == 3)
                CurrentChannel.text = "EDA";
            else if (VisualizationChannel == 4)
                CurrentChannel.text = "EEG";
            else if (VisualizationChannel == 5)
                CurrentChannel.text = "ACC";
            else if (VisualizationChannel == 6)
                CurrentChannel.text = "LUX";
            else
                CurrentChannel.text = "CH" + VisualizationChannel;
        }

        // Callback invoked once the connection with a PLUX device was established.
        public void ConnectionDone()
        {
            // Change the color and text of "Connect" button.
            if (ConnectText.text == "Connect")
            {
                Debug.Log("Connection with device " + this.SelectedDevice + " established with success!");

                ConnectText.text = "Disconnect";
                GreenFlag.SetActive(true);
                RedFlag.SetActive(false);

                // Enable "Device Configuration" panel options.
                SamplingRateInput.interactable = true;
                ResolutionInput.interactable = true;
                ResolutionDropdown.interactable = true;

                // Enable channel selection buttons accordingly to the type of device.
                string devType = PluxDevManager.GetDeviceTypeUnity();
                if (devType == "MuscleBAN BE Plux")
                {
                    CH1Toggle.interactable = true;

                    //Clear the old options of the Dropdown menu
                    ResolutionDropdown.ClearOptions();

                    //Add the options created in the List above
                    ResolutionDropdown.AddOptions(new List<string>() { "8", "16" });
                }
                else if (devType == "BITalino")
                {
                    CH1Toggle.interactable = true;
                    CH2Toggle.interactable = true;
                    CH3Toggle.interactable = true;
                    CH4Toggle.interactable = true;
                    CH5Toggle.interactable = true;
                    CH6Toggle.interactable = true;

                    //Clear the old options of the Dropdown menu
                    ResolutionDropdown.ClearOptions();

                    //Add the options created in the List above
                    ResolutionDropdown.AddOptions(new List<string>() { "10" });
                }
                else if (devType == "biosignalsplux" || devType == "BioPlux")
                {
                    CH1Toggle.interactable = true;
                    CH2Toggle.interactable = true;
                    CH3Toggle.interactable = true;
                    CH4Toggle.interactable = true;
                    CH5Toggle.interactable = true;
                    CH6Toggle.interactable = true;
                    CH7Toggle.interactable = true;
                    CH8Toggle.interactable = true;

                    //Clear the old options of the Dropdown menu
                    ResolutionDropdown.ClearOptions();

                    //Add the options created in the List above
                    if (devType == "biosignalsplux")
                    {
                        ResolutionDropdown.AddOptions(new List<string>() { "8", "16" });
                        ResolutionDropDownOptions = new List<string>() { "8", "16" };
                    }
                    else
                    {
                        ResolutionDropdown.AddOptions(new List<string>() { "8", "12" });
                        ResolutionDropDownOptions = new List<string>() { "8", "12" };
                    }
                }
                else if (devType == "OpenBANPlux")
                {
                    CH1Toggle.interactable = true;
                    CH2Toggle.interactable = true;

                    //Clear the old options of the Dropdown menu
                    ResolutionDropdown.ClearOptions();

                    //Add the options created in the List above
                    ResolutionDropdown.AddOptions(new List<string>() { "8", "16" });
                }
                else
                {
                    throw new NotSupportedException();
                }

                // Enable Start and Device Configuration buttons.
                StartButton.interactable = true;

                // Disable Connect Button.
                //ConnectButton.interactable = false;

                // Hide show Info message if it is active.
                ConnectInfoPanel.SetActive(false);

                // Update Battery Level.
                int batteryLevel = -1;
                if (devType != "BioPlux")
                {
                    batteryLevel = PluxDevManager.GetBatteryUnity();
                }

                // Battery icon accordingly to the battery level.
                List<GameObject> ListBatteryIcons = new List<GameObject>() { BatteryIcon0, BatteryIcon10, BatteryIcon50, BatteryIcon100, BatteryIconUnknown };
                GameObject currImage;
                if (batteryLevel > 50)
                {
                    BatteryIcon100.SetActive(true);
                    currImage = BatteryIcon100;
                }
                else if (batteryLevel <= 50 && batteryLevel > 10)
                {
                    BatteryIcon50.SetActive(true);
                    currImage = BatteryIcon50;
                }
                else if (batteryLevel <= 10 && batteryLevel > 1)
                {
                    BatteryIcon10.SetActive(true);
                    currImage = BatteryIcon10;
                }
                else if (batteryLevel == 0)
                {
                    BatteryIcon0.SetActive(true);
                    currImage = BatteryIcon0;
                }
                else
                {
                    BatteryIconUnknown.SetActive(true);
                    currImage = BatteryIconUnknown;
                }

                // Disable the remaining images.
                foreach (var batImg in ListBatteryIcons)
                {
                    if (batImg != currImage)
                    {
                        batImg.SetActive(false);
                    }
                }

                // Show the quantitative battery value.
                if (batteryLevel != -1)
                {
                    BatteryLevel.text = batteryLevel.ToString() + "%";
                }
                else
                {
                    BatteryLevel.text = "N.A.";
                }
            }
        }

        // Coroutine used to fade out info messages after x seconds.
        IEnumerator RemoveAfterSeconds(int seconds, GameObject obj)
        {
            yield return new WaitForSeconds(seconds);
            obj.SetActive(false);
        }

        // Function used to subsample acquired data.
        public List<int> GetSubSampleList(int[] originalArray, int samplingRate, int graphWindowSize)
        {
            // Subsampling if sampling rate is bigger than 100 Hz.
            List<int> subSamplingList = new List<int>();
            int subSamplingLevel = 1;
            if (samplingRate > 100)
            {
                // Subsampling Level.
                subSamplingLevel = samplingRate / 100;
                for (int i = 0; i < originalArray.Length; i++)
                {
                    if (i % subSamplingLevel == 0)
                    {
                        subSamplingList.Add(originalArray[i]);
                    }
                }
            }
            else
            {
                for (int i = 0; i < originalArray.Length; i++)
                {
                    subSamplingList.Add(originalArray[i]);
                }
            }

            return subSamplingList;
        }

        public void RebootVariables()
        {
            MultiThreadList = new List<List<int>>();
            ActiveChannels = new List<int>();
            MultiThreadSubList = null;
            LastLenMultiThreadString = 0;
            GraphWindSize = -1;
            VisualizationChannel = -1;
            UpdatePlotFlag = false;
            //////////////////////////////// Edit by Lillian Fan//////////////////////////////
            UpdatePlotFlag2 = false;
            EMGUpdate = false;
            ECGUpdate = false;
            EDAUpdate = false;
            //////////////////////////////////////////////////////////////////////////////////
        }

        public void OnWaitingTimeEnds(object source, ElapsedEventArgs e)
        {
            // Update flag, which will trigger the update of real-time plot.
            UpdatePlotFlag = true;
            //////////////////////////////// Edit by Lillian Fan//////////////////////////////
            UpdatePlotFlag2 = true;
            EMGUpdate = true;
            ECGUpdate = true;
            EDAUpdate = true;
            /////////////////////////////////////////////////////////////////////////////////
        }

        // Get the number of active toggle buttons.
        private int GetNbrActiveToggle()
        {
            // Number of Active Channels.
            int nbrChannels = 0;
            Toggle[] toggleArray = new Toggle[] { CH1Toggle, CH2Toggle, CH3Toggle, CH4Toggle, CH5Toggle, CH6Toggle, CH7Toggle, CH8Toggle };
            for (int i = 0; i < toggleArray.Length; i++)
            {
                if (toggleArray[i].isOn == true)
                {
                    nbrChannels++;
                }
            }

            return nbrChannels;
        }

        //////////////////////////////// Edit by Lillian Fan//////////////////////////////
        // Out Put Data
        public void OutPutEMG()
        {
            // Create Recording File
            string path = FolderPath + "/EMGData.csv";
            if (!File.Exists(path))
                File.WriteAllText(path,EMGData);
        }
        public void OutPutECG()
        {
            // Create Recording File
            string path = FolderPath + "/ECGData.csv";
            if (!File.Exists(path))
                File.WriteAllText(path, ECGData);
            path = FolderPath + "/ECGHR.csv";
            if (!File.Exists(path))
                File.WriteAllText(path,ECGHR);
        }
        public void OutPutEDA()
        {
            // Create Recording File
            string path = FolderPath + "/EDAData.csv";
            if (!File.Exists(path))
                File.WriteAllText(path,  EDAData);
        }
        ///////////////////////////////////////////////////////////////////////////////////
        ///

        //////////////////////////////// Edit by Kody Wood//////////////////////////////

        //Record EMG
        private void RecordEMG(int[] packageOfDataEMG)
        {
            string recordType = " ";
            if (eventManager.GetComponent<EventManager>().isFirstRecording)
            {
                recordType = "Baseline:";
            }
            else if (eventManager.GetComponent<EventManager>().recordMetrics)
            {
                recordType = "Adaptive:";
            }

            for (int i = 0; i < packageOfDataEMG.Length; i++)
            {
                if (i % 10 == 0)
                {
                    // resualt will between -1.64mV to 1.64mV
                    double tmp = (packageOfDataEMG[i] / Math.Pow(2, 10) - 0.5) * 3.3 / 1009 * 1000;

                    // calculate the RMS
                    EMGDataList.Add(tmp);
                    double RMS = 0;

                    if (EMGDataList.Count() >= RMSSampleRate)
                    {
                        for (int e = 0; e < RMSSampleRate; e++)
                        {
                            RMS += Math.Pow(EMGDataList[EMGDataList.Count() - 1 - e], 2);
                        }
                        RMS = RMS / RMSSampleRate;
                        RMS = Math.Sqrt(RMS);
                        RMSDataList.Add(RMS);
                    }

                    EMGData += recordType + "," + DateTime.Now.ToString("hh-mm-ss")+ "," + String.Format("{0:0.0000}", tmp) + "," + String.Format("{0:0.0000}", RMS) + "\n";
                }
            }

            EMGUpdate = false;

            //Lillian's project
            //if (RMSDataList.Count() > 0)
            //{
            //    if (RMSDataList[RMSDataList.Count() - 1] > thoresholdEMG)
            //    {
            //        isFlashing = true;
            //        MenuManage.stressTimeEMG++;
            //    }
            //}
        }

        private void RecordEDA(int[] packageOfDataEDA)
        {
            string recordType = " ";
            if (eventManager.GetComponent<EventManager>().isFirstRecording)
            {
                recordType = "Baseline:";
            }
            else if (eventManager.GetComponent<EventManager>().recordMetrics)
            {
                recordType = "Adaptive:";
            }

            //https://bitalino.com/storage/uploads/media/electrodermal-activity-eda-user-manual.pdf page 5 is the formula
            if (packageOfDataEDA.Length != 0)
            {

                for (int i = 0; i < packageOfDataEDA.Length; i++)
                {

                        //Result will be between 0uS to 25uS
                        //Transfer function
                        double tmp = packageOfDataEDA[i] / Math.Pow(2, 10) * 3.3 / 0.12;
                        
                        EDAData += recordType + "," + DateTime.Now.ToString("hh-mm-ss") + "," + String.Format("{0:0.0000}", tmp) + "\n";
                        EDANow.Add(tmp);
                        EDADataCalc.Add(tmp);
                }
            }
            EDAUpdate = false;

            //Tranfer to calculator EDANow


            //Lillian's project
            //// EDA detection
            //if (sampleingCount == 5)
            //{
            //    if (EDAPre.Count != 0)
            //    {
            //        if (EDANow.Average() - EDAPre.Average() > thoresholdEDA)
            //        {
            //            isFlashing = true;
            //            MenuManage.stressTimeEDA++;
            //        }
            //    }
            //    EDAPre.Clear();
            //    EDAPre = EDANow.ToList();
            //    EDANow.Clear();
            //    sampleingCount = 1;
            //
            //}
            //else
            //    sampleingCount++;
        }

        private void RecordECG(int[] packageOfDataECG)
        {
            string recordType = " ";
            if (eventManager.GetComponent<EventManager>().isFirstRecording)
            {
                recordType = "Baseline:";
            }
            else if (eventManager.GetComponent<EventManager>().recordMetrics)
            {
                recordType = "Adaptive:";
            }
            // variable for calculate heart rate
            double[] tmpAr = new double[0];

            if (packageOfDataECG.Length != 0)
            {
                for (int i = 0; i < packageOfDataECG.Length; i++)
                {


                        // result will between -1.5mV to 1.5mV
                        //Transfer function
                        double tmp = (packageOfDataECG[i] / Math.Pow(2, 10) - 0.5) * 3.3 / 1100 * 1000;
                        
                        //Record data to be sent to calculator
                        ECGDataCalc.Add(tmp);
                        //Calculate HR
                        ECGDataHRCalc.Add((float)tmp);
                        //recording data
                        ECGData += recordType + "," + DateTime.Now.ToString("hh-mm-ss") + "," + String.Format("{0:0.0000}", tmp) + "\n";

                        // data for calculate heart rate
                        Array.Resize(ref tmpAr, tmpAr.Length + 1);
                        tmpAr[tmpAr.Length - 1] = tmp;
                    
                }
            }



            // Calculating heart rate
            //var tmpList = new List<double>();
            //tmpList.AddRange(HBtens);
            //tmpList.AddRange(tmpAr);
            //HBtens = tmpList.ToArray();
            //if (HBCount == 10)
            //{
            //    int Heartrate = 0;
            //
            //    for (int i = 0; i < HBtens.Length - 4; i++)
            //    {
            //        if ((HBtens[i] < HBtens[i + 1]) && (HBtens[i + 1] < HBtens[i + 2]) && (HBtens[i + 2] < HBtens[i + 3]) && (HBtens[i + 3] > HBtens[i + 4]) && (HBtens[i + 3] - HBtens[i] > 0.15))
            //        {
            //            Heartrate++;
            //        }
            //    }
            //    Heartrate *= 6;
            //    //Output to screen
            //    HRText.text = "Heart Rate: " + Heartrate;
            //    //Add to the string to be outputed to CSV
            //    ECGHR += DateTime.Now.ToString("hh-mm-ss") + Heartrate + "\n";
            //
            //    //Resetting Variables
            //    HBtens = new double[] { };
            //    HBCount = 1;
            //}
            //else
            //    HBCount++;
            ECGUpdate = false;
        }

        private void CalculateHeartRate(List<float> ECGData)
        {
            string recordType = " ";
            if (eventManager.GetComponent<EventManager>().isFirstRecording)
            {
                recordType = "Baseline:";
            }
            else if (eventManager.GetComponent<EventManager>().recordMetrics)
            {
                recordType = "Adaptive:";
            }

            //Check QRS complex
            //Find maximum point of the R point
            //The time between two R points = heart rate
            //HR = 1 / (t(pointi-1) + t(pointi))
            //L = a * max|ECG(t)| , a =0.6
            //max|ECG(t)| = signals value at a certain time within a certain period of time
            //Cycle through all values of L to find the highest one. The highest value is the R peak
            //Do this a couple time and find the time at which both peak occur and then do the HR equations to find the heart rate
            //Substituting tpi for one time and tpi-1  with the other
            //https://www.wikihow.com/Calculate-Heart-Rate-from-ECG

            //Calculate Heart rate based on R peaks
            float a = 0.6f;
            float maxECG = Mathf.Max(ECGDataHRCalc.ToArray());
            float L = a * maxECG;
            int counter = 0; // Counter how many peaks were found
            float p1 = 0.0f; //Time of the first peak
            float p2 = 0.0f; //Time of the second peak
            
            float tempHR = 0.0f;
            List <float> peaks = new List<float> { };


            //Compare the L to the ecg values to find R peaks
            // We start at 1 and end 1 short to not trigger out ouf bounds error
            for (int i = 1; i < ECGDataHRCalc.Count-1; i++)
            {
                if (ECGDataHRCalc[i - 1] < L && ECGDataHRCalc[i] > L && counter == 0)
                {
                    p1 = recordTime / ECGDataHRCalc.Count * i; //RecordTime(s) / ECGDataHRCalc.Count * i sample rate 100hz = 1/100
                    counter++;

                }
                else if (ECGDataHRCalc[i - 1] > L && ECGDataHRCalc[i] < L && counter == 1)
                {
                    p2 = recordTime / ECGDataHRCalc.Count * i; //RecordTime(s) / ECGDataHRCalc.Count * i
                    counter++;

                }
                //A way to break the for loop
                if (counter == 2)
                {
                    peaks.Add((p1+p2)/2.0f);
                    counter = 0;
                    //i = ECGDataHRCalc.Count;
                }

                //Stop after 2 peaks
                //Comment this out for all peaks
                if(peaks.Count==2)
                {
                    i = ECGDataHRCalc.Count;
                }
            }

            tempHR = peaks[peaks.Count-1] - peaks[peaks.Count-2];
            heartRate = Mathf.RoundToInt((1.0f / tempHR) * 60.0f);
            ECGHR += recordType + "," + DateTime.Now.ToString("hh-mm-ss") + "," + heartRate + "\n";

            HRText.text = "Heart Rate: " + heartRate;
        }
        ///////////////////////////////////////////////////////////////////////////////////

    }
}