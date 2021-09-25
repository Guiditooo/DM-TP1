using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	private string scenePtsFinal = "PtsFinal";
	public static GameManager Instancia;
	
	public float TiempoDeJuego = 60;
	
	public enum EstadoJuego{Calibrando, Jugando, Finalizado}
	public EstadoJuego EstAct = EstadoJuego.Calibrando;
	
	public PlayerInfo PlayerInfo1 = null;
	public PlayerInfo PlayerInfo2 = null;
	
	public Player Player1;
	public Player Player2;
	
	public Transform Esqueleto1;
	public Transform Esqueleto2;
	public Vector3[] PosEsqsCarrera;
	
	bool PosSeteada = false;
	
	bool ConteoRedresivo = true;
	public Rect ConteoPosEsc;
	public float ConteoParaInicion = 3;
	public GUISkin GS_ConteoInicio;
	
	public Rect TiempoGUI = new Rect();
	public GUISkin GS_TiempoGUI;
	Rect R = new Rect();
	
	public float TiempEspMuestraPts = 3;
	
	public Vector3[]PosCamionesCarrera = new Vector3[2];

	public Vector3 PosCamion1Tuto = Vector3.zero;
	public Vector3 PosCamion2Tuto = Vector3.zero;
	

	public GameObject[] ObjsCalibracion1;
	public GameObject[] ObjsCalibracion2;

	public GameObject[] ObjsTuto1;
	public GameObject[] ObjsTuto2;

	public GameObject[] ObjsCarrera;

	
	IList<int> users;

	private bool isMultiplayer;
	
	//--------------------------------------------------------//
	
	void Awake()
	{
		GameManager.Instancia = this;
	}
	
	void Start()
	{
		IniciarCalibracion();

		switch (GameManagerX.Get().gameMode)
		{
			case GameManagerX.GameMode.Single:
				isMultiplayer = false; 
				break;
			case GameManagerX.GameMode.Multi:
				isMultiplayer = true;
				break;
		}
	}

	void Update()
	{
		if (isMultiplayer)
		{

			if (Input.GetKey(KeyCode.Mouse1) && Input.GetKey(KeyCode.Keypad0))
			{
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			}

			//CIERRA LA APLICACION
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				Application.Quit();
			}

			switch (EstAct)
			{
				case EstadoJuego.Calibrando:
					if (Input.GetKey(KeyCode.Mouse0) && Input.GetKey(KeyCode.Keypad0))
					{
						if (PlayerInfo1 != null && PlayerInfo2 != null)
						{
							FinCalibracion(0);
							FinCalibracion(1);

							FinTutorial(0);
							FinTutorial(1);
						}
					}

					if (PlayerInfo1.PJ == null && Input.GetKeyDown(KeyCode.W))
					{
						PlayerInfo1 = new PlayerInfo(0, Player1);
						PlayerInfo1.LadoAct = Visualizacion.Lado.Izq;
						SetPosicion(PlayerInfo1);
					}

					if (PlayerInfo2.PJ == null && Input.GetKeyDown(KeyCode.UpArrow))
					{
						PlayerInfo2 = new PlayerInfo(1, Player2);
						PlayerInfo2.LadoAct = Visualizacion.Lado.Der;
						SetPosicion(PlayerInfo2);
					}

					if (PlayerInfo1.PJ != null && PlayerInfo2.PJ != null)
					{
						if (PlayerInfo1.FinTuto2 && PlayerInfo2.FinTuto2)
						{
							EmpezarCarrera();
						}
					}
					break;

				case EstadoJuego.Jugando:
					if (Input.GetKey(KeyCode.Mouse1) &&
					   Input.GetKey(KeyCode.Keypad0))
					{
						TiempoDeJuego = 0;
					}

					if (TiempoDeJuego <= 0)
					{
						FinalizarCarrera();
					}

					if (ConteoRedresivo)
					{
						ConteoParaInicion -= Time.deltaTime;
						if (ConteoParaInicion < 0)
						{
							EmpezarCarrera();
							ConteoRedresivo = false;
						}
					}
					else
					{
						TiempoDeJuego -= Time.deltaTime;
					}
					break;

				case EstadoJuego.Finalizado:
					TiempEspMuestraPts -= Time.deltaTime;
					if (TiempEspMuestraPts <= 0)
						SceneManager.LoadScene(scenePtsFinal);
					break;
			}
		}
		else
		{


			//REINICIAR
			if (Input.GetKey(KeyCode.Mouse1) && Input.GetKey(KeyCode.Keypad0))
			{
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			}

			//CIERRA LA APLICACION
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				Application.Quit();
			}


			switch (EstAct)
			{
				case EstadoJuego.Calibrando:

					//SKIP EL TUTORIAL
					if (Input.GetKey(KeyCode.Mouse0) && Input.GetKey(KeyCode.Keypad0))
					{
						if (PlayerInfo1 != null)
						{
							FinCalibracion(0);
							FinTutorial(0);
						}
					}

					if (PlayerInfo1.PJ == null && Input.GetKeyDown(KeyCode.W))
					{
						PlayerInfo1 = new PlayerInfo(0, Player1);
						PlayerInfo1.LadoAct = Visualizacion.Lado.Izq;
						SetPosicion(PlayerInfo1);
					}

					if (PlayerInfo1.PJ != null)
					{
						if (PlayerInfo1.FinTuto2)
						{
							EmpezarCarrera();
						}
					}

					break;

				case EstadoJuego.Jugando:

					//SKIP LA CARRERA
					if (Input.GetKey(KeyCode.Mouse1) && Input.GetKey(KeyCode.Keypad0))
					{
						TiempoDeJuego = 0;
					}

					if (TiempoDeJuego <= 0)
					{
						FinalizarCarrera();
					}

					if (ConteoRedresivo)
					{
						ConteoParaInicion -= Time.deltaTime;
						if (ConteoParaInicion < 0)
						{
							EmpezarCarrera();
							ConteoRedresivo = false;
						}
					}
					else
					{
						TiempoDeJuego -= Time.deltaTime;
					}
					break;

				case EstadoJuego.Finalizado:
					TiempEspMuestraPts -= Time.deltaTime;
					if (TiempEspMuestraPts <= 0)
						SceneManager.LoadScene(scenePtsFinal);
					break;
			}

		}

	}
	
	void OnGUI()
	{
		switch (EstAct)
		{
		case EstadoJuego.Jugando:
			if(ConteoRedresivo)
			{
				GUI.skin = GS_ConteoInicio;
				
				R.x = ConteoPosEsc.x * Screen.width/100;
				R.y = ConteoPosEsc.y * Screen.height/100;
				R.width = ConteoPosEsc.width * Screen.width/100;
				R.height = ConteoPosEsc.height * Screen.height/100;
				
				if(ConteoParaInicion > 1)
				{
					GUI.Box(R, ConteoParaInicion.ToString("0"));
				}
				else
				{
					GUI.Box(R, "GO");
				}
			}
			
			GUI.skin = GS_TiempoGUI;
			R.x = TiempoGUI.x * Screen.width/100;
			R.y = TiempoGUI.y * Screen.height/100;
			R.width = TiempoGUI.width * Screen.width/100;
			R.height = TiempoGUI.height * Screen.height/100;
			GUI.Box(R,TiempoDeJuego.ToString("00"));
			break;
		}
		
		GUI.skin = null;
	}
	
	//----------------------------------------------------------//
	
	public void IniciarCalibracion()
	{
		if (isMultiplayer)
		{
			for (int i = 0; i < ObjsCalibracion1.Length; i++)
			{
				ObjsCalibracion1[i].SetActive(true);
				ObjsCalibracion2[i].SetActive(true);
			}

			for (int i = 0; i < ObjsTuto2.Length; i++)
			{
				ObjsTuto2[i].SetActive(false);
				ObjsTuto1[i].SetActive(false);
			}

			for (int i = 0; i < ObjsCarrera.Length; i++)
			{
				ObjsCarrera[i].SetActive(false);
			}


			Player1.CambiarACalibracion();
			Player2.CambiarACalibracion();
		}
        else 
		{
			for (int i = 0; i < ObjsCalibracion1.Length; i++)
			{
				ObjsCalibracion1[i].SetActive(true);
			}

			for (int i = 0; i < ObjsTuto2.Length; i++)
			{
				ObjsTuto2[i].SetActive(false);
			}

			for (int i = 0; i < ObjsCarrera.Length; i++)
			{
				ObjsCarrera[i].SetActive(false);
			}


			Player1.CambiarACalibracion();
		}
	}	
	
	void CambiarATutorial()
	{
		if (isMultiplayer)
		{
			PlayerInfo1.FinCalibrado = true;

			for (int i = 0; i < ObjsTuto1.Length; i++)
			{
				ObjsTuto1[i].SetActive(true);
			}

			for (int i = 0; i < ObjsCalibracion1.Length; i++)
			{
				ObjsCalibracion1[i].SetActive(false);
			}
			Player1.GetComponent<Frenado>().Frenar();
			Player1.CambiarATutorial();
			Player1.gameObject.transform.position = PosCamion1Tuto;//posiciona el camion
			Player1.transform.forward = Vector3.forward;


			PlayerInfo2.FinCalibrado = true;

			for (int i = 0; i < ObjsCalibracion2.Length; i++)
			{
				ObjsCalibracion2[i].SetActive(false);
			}

			for (int i = 0; i < ObjsTuto2.Length; i++)
			{
				ObjsTuto2[i].SetActive(true);
			}
			Player2.GetComponent<Frenado>().Frenar();
			Player2.gameObject.transform.position = PosCamion2Tuto;
			Player2.CambiarATutorial();
			Player2.transform.forward = Vector3.forward;
		}
		else
        {
			PlayerInfo1.FinCalibrado = true;

			for (int i = 0; i < ObjsTuto1.Length; i++)
			{
				ObjsTuto1[i].SetActive(true);
			}

			for (int i = 0; i < ObjsCalibracion1.Length; i++)
			{
				ObjsCalibracion1[i].SetActive(false);
			}
			Player1.GetComponent<Frenado>().Frenar();
			Player1.CambiarATutorial();
			Player1.gameObject.transform.position = PosCamion1Tuto;//posiciona el camion
			Player1.transform.forward = Vector3.forward;

		}
	}
	
	void EmpezarCarrera()
	{
		if (isMultiplayer)
		{
			Player1.GetComponent<Frenado>().RestaurarVel();
			Player1.GetComponent<ControlDireccion>().Habilitado = true;

			Player2.GetComponent<Frenado>().RestaurarVel();
			Player2.GetComponent<ControlDireccion>().Habilitado = true;
		}
		else
        {
			Player1.GetComponent<Frenado>().RestaurarVel();
			Player1.GetComponent<ControlDireccion>().Habilitado = true;
		}
	}
	
	void FinalizarCarrera()
	{
		if (isMultiplayer)
		{
			EstAct = GameManager.EstadoJuego.Finalizado;

			TiempoDeJuego = 0;

			if (Player1.Dinero > Player2.Dinero)
			{
				//puntajes
				DatosPartida.PtsGanador = Player1.Dinero;
				DatosPartida.PtsPerdedor = Player2.Dinero;
			}
			else
			{
				//puntajes
				DatosPartida.PtsGanador = Player2.Dinero;
				DatosPartida.PtsPerdedor = Player1.Dinero;
			}

			DatosPartida.LadoGanadaor = PlayerInfo2.LadoAct == Visualizacion.Lado.Der ? DatosPartida.Lados.Der : DatosPartida.Lados.Izq;

			Player1.GetComponent<Frenado>().Frenar();
			Player2.GetComponent<Frenado>().Frenar();

			Player1.ContrDesc.FinDelJuego();
			Player2.ContrDesc.FinDelJuego();
		}
		else
        {
			EstAct = EstadoJuego.Finalizado;

			TiempoDeJuego = 0;

			DatosPartida.LadoGanadaor = PlayerInfo2.LadoAct == Visualizacion.Lado.Der ? DatosPartida.Lados.Der : DatosPartida.Lados.Izq;

			if (Player1.Dinero > Player2.Dinero)
			{
				//puntajes
				DatosPartida.PtsGanador = Player1.Dinero;
				DatosPartida.PtsPerdedor = Player2.Dinero;
			}
			else
			{
				//puntajes
				DatosPartida.PtsGanador = Player2.Dinero;
				DatosPartida.PtsPerdedor = Player1.Dinero;
			}

			Player1.GetComponent<Frenado>().Frenar();

			Player1.ContrDesc.FinDelJuego();
		}
	}
	
	//se encarga de posicionar la camara derecha para el jugador que esta a la derecha y viseversa
	void SetPosicion(PlayerInfo pjInf)
	{	
		pjInf.PJ.GetComponent<Visualizacion>().SetLado(pjInf.LadoAct);
		//en este momento, solo la primera vez, deberia setear la otra camara asi no se superponen
		pjInf.PJ.ContrCalib.IniciarTesteo();
		PosSeteada = true;
		
		
		if(pjInf.PJ == Player1)
		{
			if(pjInf.LadoAct == Visualizacion.Lado.Izq)
				Player2.GetComponent<Visualizacion>().SetLado(Visualizacion.Lado.Der);
			else
				Player2.GetComponent<Visualizacion>().SetLado(Visualizacion.Lado.Izq);
		}
		else
		{
			if(pjInf.LadoAct == Visualizacion.Lado.Izq)
				Player1.GetComponent<Visualizacion>().SetLado(Visualizacion.Lado.Der);
			else
				Player1.GetComponent<Visualizacion>().SetLado(Visualizacion.Lado.Izq);
		}
		
	}
	
	void CambiarACarrera()
	{
		if (isMultiplayer)
		{
			Esqueleto1.transform.position = PosEsqsCarrera[0];
			Esqueleto2.transform.position = PosEsqsCarrera[1];

			for (int i = 0; i < ObjsCarrera.Length; i++)
			{
				ObjsCarrera[i].SetActive(true);
			}

			PlayerInfo1.FinCalibrado = true;

			for (int i = 0; i < ObjsTuto1.Length; i++)
			{
				ObjsTuto1[i].SetActive(true);
			}

			for (int i = 0; i < ObjsCalibracion1.Length; i++)
			{
				ObjsCalibracion1[i].SetActive(false);
			}

			PlayerInfo2.FinCalibrado = true;

			for (int i = 0; i < ObjsCalibracion2.Length; i++)
			{
				ObjsCalibracion2[i].SetActive(false);
			}

			for (int i = 0; i < ObjsTuto2.Length; i++)
			{
				ObjsTuto2[i].SetActive(true);
			}

			if (PlayerInfo1.LadoAct == Visualizacion.Lado.Izq)
			{
				Player1.gameObject.transform.position = PosCamionesCarrera[0];
				Player2.gameObject.transform.position = PosCamionesCarrera[1];
			}
			else
			{
				Player1.gameObject.transform.position = PosCamionesCarrera[1];
				Player2.gameObject.transform.position = PosCamionesCarrera[0];
			}

			Player1.transform.forward = Vector3.forward;
			Player1.GetComponent<Frenado>().Frenar();
			Player1.CambiarAConduccion();

			Player2.transform.forward = Vector3.forward;
			Player2.GetComponent<Frenado>().Frenar();
			Player2.CambiarAConduccion();

			//los deja andando
			Player1.GetComponent<Frenado>().RestaurarVel();
			Player2.GetComponent<Frenado>().RestaurarVel();
			//cancela la direccion
			Player1.GetComponent<ControlDireccion>().Habilitado = false;
			Player2.GetComponent<ControlDireccion>().Habilitado = false;
			//les de direccion
			Player1.transform.forward = Vector3.forward;
			Player2.transform.forward = Vector3.forward;

			EstAct = GameManager.EstadoJuego.Jugando;

		}
		else
        {
			Esqueleto1.transform.position = PosEsqsCarrera[0];

			for (int i = 0; i < ObjsCarrera.Length; i++)
			{
				ObjsCarrera[i].SetActive(true);
			}

			PlayerInfo1.FinCalibrado = true;

			for (int i = 0; i < ObjsTuto1.Length; i++)
			{
				ObjsTuto1[i].SetActive(true);
			}

			for (int i = 0; i < ObjsCalibracion1.Length; i++)
			{
				ObjsCalibracion1[i].SetActive(false);
			}

			//posiciona los camiones dependiendo de que lado de la pantalla esten
			if (PlayerInfo1.LadoAct == Visualizacion.Lado.Izq)
			{
				Player1.gameObject.transform.position = PosCamionesCarrera[0];
			}
			else
			{
				Player1.gameObject.transform.position = PosCamionesCarrera[1];
			}

			Player1.transform.forward = Vector3.forward;
			Player1.GetComponent<Frenado>().Frenar();
			Player1.CambiarAConduccion();

			//los deja andando
			Player1.GetComponent<Frenado>().RestaurarVel();
			//cancela la direccion
			Player1.GetComponent<ControlDireccion>().Habilitado = false;
			//les de direccion
			Player1.transform.forward = Vector3.forward;

			EstAct = EstadoJuego.Jugando;
		}
	}
	
	public void FinTutorial(int playerID)
	{
		if (isMultiplayer)
		{
			if (playerID == 0)
			{
				PlayerInfo1.FinTuto2 = true;

			}
			else if (playerID == 1)
			{
				PlayerInfo2.FinTuto2 = true;
			}

			if (PlayerInfo1.FinTuto2 && PlayerInfo2.FinTuto2)
			{
				CambiarACarrera();
			}
		}
		else
        {
			if (playerID == 0)
			{
				PlayerInfo1.FinTuto2 = true;

			}

			if (PlayerInfo1.FinTuto2)
			{
				CambiarACarrera();
			}
		}
	}
	
	public void FinCalibracion(int playerID)
	{
		if (isMultiplayer)
		{
			if (playerID == 0)
			{
				PlayerInfo1.FinTuto1 = true;

			}
			else if (playerID == 1)
			{
				PlayerInfo2.FinTuto1 = true;
			}

			if (PlayerInfo1.PJ != null && PlayerInfo2.PJ != null)
				if (PlayerInfo1.FinTuto1 && PlayerInfo2.FinTuto1)
					CambiarACarrera();//CambiarATutorial();
		}
		else
		{
			if (playerID == 0)
			{
				PlayerInfo1.FinTuto1 = true;
			}

			if (PlayerInfo1.PJ != null && PlayerInfo1.FinTuto1)
			{
				CambiarACarrera();
			}
		}
	}
	
	
	
	
	[System.Serializable]
	public class PlayerInfo
	{
		public PlayerInfo(int tipoDeInput, Player pj)
		{
            TipoDeInput = tipoDeInput;
			PJ = pj;
		}
		
		public bool FinCalibrado = false;
		public bool FinTuto1 = false;
		public bool FinTuto2 = false;
		
		public Visualizacion.Lado LadoAct;

        public int TipoDeInput = -1;
		
		public Player PJ;
	}
	
}
