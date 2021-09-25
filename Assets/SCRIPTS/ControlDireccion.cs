using UnityEngine;
using System.Collections;

public class ControlDireccion : MonoBehaviour 
{
	public enum TipoInput {Mouse, Kinect, AWSD, Arrows}
	public TipoInput InputAct = ControlDireccion.TipoInput.Mouse;

	public Transform ManoDer;
	public Transform ManoIzq;

	CarController carController;
	
	public float MaxAng = 90;
	public float DesSencibilidad = 90;
	
	float Giro = 0;
	
	public enum Sentido {Der, Izq}
	Sentido DirAct;
	
	public bool Habilitado = true;
		
	//---------------------------------------------------------//
	
	// Use this for initialization
	void Awake () 
	{
		carController = GetComponent<CarController>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		switch(InputAct)
		{
		case TipoInput.Mouse:
				if (Habilitado)
					carController.SetGiro(MousePos.Relation(MousePos.AxisRelation.Horizontal));//debe ser reemplanado
			break;
			
		case TipoInput.Kinect:

			DirAct = (ManoIzq.position.y > ManoDer.position.y) ? Sentido.Der : Sentido.Der;
			
			switch(DirAct)
			{
			case Sentido.Der:
				Giro = (Angulo() <= MaxAng) ? (Angulo() / (MaxAng + DesSencibilidad)) : 1;
				if (Habilitado)
					carController.SetGiro(Giro);
				break;
				
			case Sentido.Izq:
				Giro = (Angulo() <= MaxAng) ? (Angulo() / (MaxAng + DesSencibilidad)) * (-1) : (-1);
				if(Habilitado)
					carController.SetGiro(Giro);
				break;
			}
		break;
           
		case TipoInput.AWSD:
            if (Habilitado) {
                if (Input.GetKey(KeyCode.A)) 
				{
					carController.SetGiro(-1);
                }
                if (Input.GetKey(KeyCode.D))
				{
					carController.SetGiro(1);
                }
            }
            break;
        case TipoInput.Arrows:
            if (Habilitado) {
                if (Input.GetKey(KeyCode.LeftArrow)) 
				{
					carController.SetGiro(-1);
                }
                if (Input.GetKey(KeyCode.RightArrow))
				{
					carController.SetGiro(1);
                }
            }
            break;
        }		
	}

	public float GetGiro()
	{
		
		return Giro;
	}
	
	float Angulo()
	{
		Vector2 diferencia = new Vector2(ManoDer.localPosition.x, ManoDer.localPosition.y) - new Vector2(ManoIzq.localPosition.x, ManoIzq.localPosition.y);
		return Vector2.Angle(diferencia,new Vector2(1,0));
	}
	
}
