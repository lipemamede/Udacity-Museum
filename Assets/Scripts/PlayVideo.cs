using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class PlayVideo : MonoBehaviour {

	private int noOfDisplay = 6;
	public VideoPlayer[] videoplayers;
	public GameObject[] playButtons;
	public Material playMaterial;
	public Material pauseMaterial;
	public Canvas sliderCanvas;
	public Slider slider;
	public Slider startSlider;
	public GameObject welcomeScreen;
	public AudioSource startScreenAudio;

	private float enterTime;
	private bool pointerFlag = false;
	private int buttonIndex;
	private int startScreenBtnIndex;
	private string pointedObj;
	private AudioSource playBtnAudio;

	private Vector3 pausePos = new Vector3 (0f, -0.5f, 0f);
	private Vector3 playPos = new Vector3 (0f, 0.5f, 0f);

	// Use this for initialization
	void Start () {
		slider.maxValue = 1.5f;

		for (int i = 0; i < noOfDisplay; i++) {
			GameObject display = videoplayers [i].transform.parent.GetChild(1).gameObject;
			display.SetActive (false);
		}
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void PointerEnter(int index) {
		pointerFlag = true;
		enterTime = Time.time;
		sliderCanvas.gameObject.SetActive (true);

		if (index >= 10) {
			pointedObj = "welcomeScreenBtn";
			startScreenBtnIndex = index;
			startSlider.gameObject.SetActive (true);
		} else {
			buttonIndex = index;
			pointedObj = "playBtn";
		}
	}

	public void PointerExit() {
		pointerFlag = false;
		sliderCanvas.gameObject.SetActive (false);
		startSlider.gameObject.SetActive (false);
	}


	public void StartTour(int btnIndex) {
		startScreenAudio.Play ();
		if (btnIndex == 10) {
			//welcomeScreen.transform.GetChild (0).gameObject.SetActive (false);
			welcomeScreen.transform.GetChild (1).gameObject.SetActive (true);
			startSlider.gameObject.SetActive (false);

		} else {
			welcomeScreen.SetActive (false);
			startSlider.gameObject.SetActive (false);
			sliderCanvas.gameObject.SetActive (false);
		}
	}

	//Play video on click of Play button
	public void PlayButtonClicked( int index) {
		GameObject display = videoplayers [index].transform.parent.GetChild(1).gameObject;
		display.SetActive (true);

		TogglePause (index);
	}

	//Reset each video player when user moves
	public void ResetAll() {
		for (int i = 0; i < noOfDisplay; i++) {
			GameObject display = videoplayers [i].transform.parent.GetChild(1).gameObject;
			display.SetActive (false);
			Renderer render =  playButtons[i].GetComponentsInChildren<Renderer>()[0];
			render.material = playMaterial;

			videoplayers [i].Stop ();
		}
	}

	public void TogglePause(int index) {
		if (videoplayers [index].isPlaying) {
			videoplayers [index].Pause ();
			Renderer render =  playButtons[index].GetComponentsInChildren<Renderer>()[0];
			render.material = playMaterial;

			playButtons [index].transform.position += playPos;
		} else {
			videoplayers [index].Play ();
			Renderer render =  playButtons[index].GetComponentsInChildren<Renderer>()[0];
			render.material = pauseMaterial;
		
			playButtons [index].transform.position += pausePos;
		}
		playBtnAudio =  playButtons [index].GetComponent<AudioSource> ();
		playBtnAudio.Play ();
	}



}
