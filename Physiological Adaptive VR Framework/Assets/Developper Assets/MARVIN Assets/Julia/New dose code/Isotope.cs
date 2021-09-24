using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Isotope {

    //########################Constructors########################

    public float concentration = -1f;
    public RadioIsotopes radioIsotope;

    private string name;
    private bool wasConstructed = false;

    public Isotope() {
        
        readCSV();
        
    }

    public Isotope( RadioIsotopes radioIsotope , float concentration ) {
        
        this.concentration = concentration;
        name = isotopeNames[ ( int ) radioIsotope ];

        readCSV();

        wasConstructed = true;

    }

    public Isotope( string isotopeName , float concentration ) {
        
        this.concentration = concentration;
        this.name = isotopeName;

        readCSV();

        wasConstructed = true;

    }

    //Z is the number of protons, N is the number of neutrons
    public Isotope( int Z , int N , bool meta , float concentration ) {

        this.concentration = concentration;
        
        name = isotopeSymbols[ Z - 1 ].Split( '-' )[ 1 ] + ( meta ? "m" : "" );
        
        readCSV();

        wasConstructed = true;

    }

    private void properConstruct() {

        if ( !wasConstructed ) {

            name = isotopeNames[ ( int ) radioIsotope ];
            wasConstructed = true;

        }

    }

    //########################Functions########################
    

    //Return in keV
    public float getGammaDecayEnergy() {

        properConstruct();

        if ( !isStable() ) {

            string name = getIsotopeName();

            foreach ( Dictionary<string , string> isotope in isotopeData ) {
                
                if ( isotope[ "Full name" ] == this.getIsotopeName() && isotope[ "Gamma energy (MeV)" ]  != "" ) {
                    
                    return ( float.Parse( isotope[ "Gamma energy (MeV)" ] ) * 100 );

                }

            }

        }
        
        return 0;

    }

    //Return in keV
    public float getBetaDecayEnergy() {

        properConstruct();

        if ( !isStable() ) {

            foreach ( Dictionary<string , string> isotope in isotopeData ) {

                if ( isotope[ "Full name" ] == this.getIsotopeName() && isotope[ "Beta energy (MeV)" ] != "" ) {

                    return ( float.Parse( isotope[ "Beta energy (MeV)" ] ) * 100 );

                }

            }

        }

        return 0;

    }

    //Return in s
    public float getHalfLife() {

        properConstruct();

        foreach ( Dictionary<string , string> isotope in isotopeData ) {

            if ( isotope[ "Full name" ] == this.getIsotopeName() ) {

                string units = "";

                foreach ( string key in isotope.Keys ) {

                    if ( key.Contains( "Unit" ) ) {

                        units = isotope[ key ];
                        break;

                    }

                }
                
                string rawHalfLifeString = isotope[ "Half-life" ];
                
                float rawHalfLife = float.Parse( rawHalfLifeString );

                if ( units.Contains( "yr" ) ) {
                    
                    rawHalfLife *= 31622400;

                }
                else if ( units.Contains( "mn" ) ) {

                    rawHalfLife *= 60;

                }
                else if ( units.Contains( "dy" ) ) {

                    rawHalfLife *= 864000;

                }
                
                return rawHalfLife;

            }

        }

        return 0;

    }

    public bool isStable() {

        properConstruct();
        //If its in the table its not stable

        foreach ( Dictionary<string , string> isotope in isotopeData ) {

            if ( isotope[ "Full name" ] == this.getIsotopeName() ) {

                return false;

            }

        }

        return true;

    }

    public List<Isotope> getDecayProducts() {

        properConstruct();
        List<Isotope> daughters = new List<Isotope>();

        if ( !isStable() ) {

            string name = getIsotopeName();

            foreach ( Dictionary<string , string> isotope in isotopeData ) {

                if ( isotope[ "Full name" ] == this.getIsotopeName() ) {

                    if ( isotope[ "Decay" ] == "B-" ) {

                        int Z = int.Parse( isotope[ "Z" ] );
                        int N = int.Parse( isotope[ "N" ] );
                        bool meta = isotope[ "Meta-stable daughter" ] == "TRUE";
                        float newConcentration = ( float.Parse( isotope[ "Decay probability (%)" ] ) / 100 ) * getConcentration();
                        
                        daughters.Add( new Isotope( Z + 1 , N - 1 , meta , newConcentration ) );

                    }

                }

            }

        }

        return daughters;

    }

    //########################Private Data########################

    private List<Dictionary<string , string>> isotopeData;
  
    private void readCSV() {
        
        isotopeData = new List<Dictionary<string , string>>();

        string fileData = System.IO.File.ReadAllText( "Assets/Julia/RadioNuke.csv" );
        string[] lines = fileData.Split( "\n".ToCharArray() );
        
        if ( lines.Length > 2 ) {

            string[] headers = lines[ 0 ].Split( ",".ToCharArray() );

            for ( int i = 1 ; i < lines.Length ; i++ ) {

                if ( lines[ i ] != "" ) {

                    //Splits the line by comma
                    string[] data = lines[ i ].Split( ",".ToCharArray() );

                    Dictionary<string , string> subData = new Dictionary<string , string>();

                    for ( int c = 0 ; c < headers.Length ; c++ ) {

                        subData.Add( headers[ c ] , data[ c ] );

                    }
                    
                    isotopeData.Add( subData );

                }
                
            }


        }


    }

    public float getConcentration() {

        return concentration;

    }

    public string getIsotopeName() {

        return name;

    }

    public enum RadioIsotopes {

        Co_60,
        Cs_137,
        Ba_137m,
        I_131,


    }

    private string[] isotopeNames = new string[] {

        "Co-60",
        "Cs-137",
        "Ba_137n",
        "I_131",

    };

    private string[] isotopeSymbols = new string[] {

        "1-H-Hydrogen",
        "2-He-Helium",
        "3-Li-Lithium",
        "4-Be-Beryllium",
        "5-B-Boron",
        "6-C-Carbon",
        "7-N-Nitrogen",
        "8-O-Oxygen",
        "9-F-Fluorine",
        "10-Ne-Neon",
        "11-Na-Sodium",
        "12-Mg-Magnesium",
        "13-Al-Aluminium",
        "14-Si-Silicon",
        "15-P-Phosphorus",
        "16-S-Sulfur",
        "17-Cl-Chlorine",
        "18-Ar-Argon",
        "19-K-Potassium",
        "20-Ca-Calcium",
        "21-Sc-Scandium",
        "22-Ti-Titanium",
        "23-V-Vanadium",
        "24-Cr-Chromium",
        "25-Mn-Manganese",
        "26-Fe-Iron",
        "27-Co-Cobalt",
        "28-Ni-Nickel",
        "29-Cu-Copper",
        "30-Zn-Zinc",
        "31-Ga-Gallium",
        "32-Ge-Germanium",
        "33-As-Arsenic",
        "34-Se-Selenium",
        "35-Br-Bromine",
        "36-Kr-Krypton",
        "37-Rb-Rubidium",
        "38-Sr-Strontium",
        "39-Y-Yttrium",
        "40-Zr-Zirconium",
        "41-Nb-Niobium",
        "42-Mo-Molybdenum",
        "43-Tc-Technetium",
        "44-Ru-Ruthenium",
        "45-Rh-Rhodium",
        "46-Pd-Palladium",
        "47-Ag-Silver",
        "48-Cd-Cadmium",
        "49-In-Indium",
        "50-Sn-Tin",
        "51-Sb-Antimony",
        "52-Te-Tellurium",
        "53-I-Iodine",
        "54-Xe-Xenon",
        "55-Cs-Cesium",
        "56-Ba-Barium",
        "57-La-Lanthanum",
        "58-Ce-Cerium",
        "59-Pr-Praseodymium",
        "60-Nd-Neodymium",
        "61-Pm-Promethium",
        "62-Sm-Samarium",
        "63-Eu-Europium",
        "64-Gd-Gadolinium",
        "65-Tb-Terbium",
        "66-Dy-Dysprosium",
        "67-Ho-Holmium",
        "68-Er-Erbium",
        "69-Tm-Thulium",
        "70-Yb-Ytterbium",
        "71-Lu-Lutetium",
        "72-Hf-Hafnium",
        "73-Ta-Tantalum",
        "74-W-Tungsten",
        "75-Re-Rhenium",
        "76-Os-Osmium",
        "77-Ir-Iridium",
        "78-Pt-Platinum",
        "79-Au-Gold",
        "80-Hg-Mercury",
        "81-Tl-Thallium",
        "82-Pb-Lead",
        "83-Bi-Bismuth",
        "84-Po-Polonium",
        "85-At-Astatine",
        "86-Rn-Radon",
        "87-Fr-Francium",
        "88-Ra-Radium",
        "89-Ac-Actinium",
        "90-Th-Thorium",
        "91-Pa-Protactinium",
        "92-U-Uranium",
        "93-Np-Neptunium",
        "94-Pu-Plutonium",
        "95-Am-Americium",
        "96-Cm-Curium",
        "97-Bk-Berkelium",
        "98-Cf-Californium",
        "99-Es-Einsteinium",
        "100-Fm-Fermium",
        "101-Md-Mendelevium",
        "102-No-Nobelium",
        "103-Lr-Lawrencium",
        "104-Rf-Rutherfordium",
        "105-Db-Dubnium",
        "106-Sg-Seaborgium",
        "107-Bh-Bohrium",
        "108-Hs-Hassium",
        "109-Mt-Meitnerium",
        "110-Ds-Darmstadtium",
        "111-Rg-Roentgenium",
        "112-Cn-Copernicium",
        "113-Nh-Nihonium",
        "114-Fl-Flerovium",
        "115-Mc-Moscovium",
        "116-Lv-Livermorium",
        "117-Ts-Tennessine",
        "118-Og-Oganesson"

    };

}
