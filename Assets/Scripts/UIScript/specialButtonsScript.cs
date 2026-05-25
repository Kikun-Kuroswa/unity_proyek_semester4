using UnityEngine;

public class specialButtonsScript : MonoBehaviour
{
    public movementScript movementScript; // Reference to the movement script

    public void tawuranButton(){
        movementScript.ignoreTeammatesToggle();
    }
}
