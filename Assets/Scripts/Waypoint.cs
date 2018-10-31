using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Waypoint : MonoBehaviour
{
	private enum State
	{
		Idle,
		Focused,
		Clicked,
		Approaching,
		Moving,
		Collect,
		Collected,
		Occupied,
		Open,
		Hidden
	}

	[SerializeField]
	private State  		_state					= State.Idle;
	private Color		_color_origional		= new Color(0.0f, 1.0f, 0.0f, 0.5f);
	private Color		_color					= Color.white;
	private float 		_scale					= 1.0f;
	private float 		_animated_lerp			= 1.0f;
	private AudioSource _audio_source			= null;
	private Material	_material				= null;

	[Header("Material")]
	public Material	material					= null;
	public Color color_hilight					= new Color(0.8f, 0.8f, 1.0f, 0.125f);	

	[Header("State Blend Speeds")]
	public float lerp_idle 						= 0.0f;
	public float lerp_focus 					= 0.0f;
	public float lerp_hide						= 0.0f;
	public float lerp_clicked					= 0.0f;

	[Header("State Animation Scales")]
	public float scale_clicked_max				= 0.0f;
	public float scale_animation				= 3.0f;	
	public float scale_idle_min 				= 0.0f;
	public float scale_idle_max 				= 0.0f;
	public float scale_focus_min				= 0.0f;
	public float scale_focus_max				= 0.0f;

	[Header("Sounds")]
	public AudioClip clip_click					= null;				

	[Header("Hide Distance")]
	public float threshold						= 0.125f;

	private float enterTime;
	private bool pointerFlag = false;

	public PlayVideo videoObject;

	public Canvas sliderCanvas;

	public Slider slider;

	void Awake()
	{		
		_material					= Instantiate(material);
		_color_origional			= _material.color;
		_color						= _color_origional;
		_audio_source				= gameObject.GetComponent<AudioSource>();	
		_audio_source.clip		 	= clip_click;
		_audio_source.playOnAwake 	= false;
		slider.maxValue = 1.5f;
		sliderCanvas.gameObject.SetActive (false);
	}


	void Update()
	{
		bool occupied 	= Camera.main.transform.parent.transform.position == gameObject.transform.position;

		switch(_state)
		{
		case State.Idle:
			Idle();

			_state 		= occupied ? State.Occupied : _state;
			break;

		case State.Focused:
			Focus();
			break;

		case State.Clicked:
			Clicked();

			bool scaled = _scale >= scale_clicked_max * .95f;
			_state 		= scaled ? State.Approaching : _state;
			break;

		case State.Approaching:
			Hide();	

			_state 		= occupied ? State.Occupied : _state;
			break;
		case State.Occupied:
			Hide();

			_state = !occupied ? State.Idle : _state;
			break;

		case State.Hidden:
			Hide();
			break;

		default:
			break;
		}

		gameObject.GetComponentInChildren<MeshRenderer>().material.color 	= _color;
		gameObject.transform.localScale 									= Vector3.one * _scale;

		_animated_lerp														= Mathf.Abs(Mathf.Cos(Time.time * scale_animation));

		if (pointerFlag) {
			//Sets the progress slider value
			float sliderValue = Time.time - enterTime;
			slider.value = sliderValue;

			if ((Time.time - enterTime) > slider.maxValue) {
				Click ();
				pointerFlag = false;
			}
		}
	}


	public void Enter()
	{
		_state = _state == State.Idle ? State.Focused : _state;
		pointerFlag = true;
		enterTime = Time.time;


	}


	public void Exit()
	{
		_state = State.Idle;
		pointerFlag = false;
		sliderCanvas.gameObject.SetActive (false);
	}


	public void Click()
	{
		_state = _state == State.Focused ? State.Clicked : _state;

		_audio_source.Play();

		Camera.main.transform.parent.transform.position = gameObject.transform.position;

		//Reset videoplayers when user moves
		videoObject.ResetAll ();

		//Reset progress slider value
		slider.value = 0.0f;
	}


	private void Idle()
	{
		Renderer renderer = gameObject.GetComponentInChildren<Renderer> ();
		Color emissionColor = Color.black;
		renderer.material.SetColor("_EmissionColor", emissionColor);
	}


	public void Focus()
	{
		transform.Rotate(0.0f, 100*Time.deltaTime, 0.0f, Space.Self);

		Renderer renderer = gameObject.GetComponentInChildren<Renderer> ();
		Color emissionColor = new Color (0.14f, 0.35f, 0.0f, 0.2f);
		renderer.material.SetColor("_EmissionColor", emissionColor);

		sliderCanvas.gameObject.SetActive (true);
	}


	public void Clicked()
	{	
		_scale					= Mathf.Lerp(_scale, scale_clicked_max, lerp_clicked);
		_color					= Color.Lerp(_color,     color_hilight, lerp_clicked);
	}


	public void Hide()
	{
		_scale					= Mathf.Lerp(_scale, 		0.0f, lerp_hide);
		_color					= Color.Lerp(_color, Color.clear, lerp_hide);
	}
}
