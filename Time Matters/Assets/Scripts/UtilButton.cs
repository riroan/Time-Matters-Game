using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilButton : MonoBehaviour
{ 
    [SerializeField] GameObject Bttn; // 매뉴 누르면 서브 메뉴 칸 나타난다.
    [SerializeField] GameObject Bttn_comp; // 매뉴 누르면 서브 메뉴 칸 나타난다.
    [SerializeField] GameObject Button_continue; // 버튼 누르면 다시 원래대로
   // [SerializeField] GameObject Button_setting; // 세팅 창으로 간다
   // [SerializeField] GameObject Button_exit; // 원래 본 화면으로 간다.
   // [SerializeField] GameObject insettings; // 인세팅 메뉴로 이동한다. 그곳에서 설정 가능하게 한다
   // [SerializeField] GameObject mainpage; // 맨 처음 메인 화면으로 되돌아간다.


    public void subButtonClick() // 왼쪽 위에 있는 메뉴 클릭 함수
    {
        Bttn_comp.SetActive(true);
    }

    public void Btutton_continue_Clicked() // 계속하기 버튼을 누를 때의 함수
    {
        Bttn_comp.SetActive(false);

    }

    public void settings_btn_Clicked() // 설정 버튼을 누를 때의 함수
    {
        //insettings.gameObject.SetActive(true);
    }

    public void mainmenu_btn_Clicked() // 메인메뉴 버튼을 누를 때의 함수
    {
        //mainpage.gameObject.SetActive(true);
    }
   
}
