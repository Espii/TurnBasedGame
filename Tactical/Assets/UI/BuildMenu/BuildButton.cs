using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class BuildButton : MonoBehaviour, IPointerClickHandler {
    public BuildMenu menu;
    Entity content;
    public Image ImageContainer;
    public Image image;
    public Text name;
    public Text cost;
    public Text info0;
    public Text info1;

    public void SetContent(Entity content)
    {
        this.content = content;
        if (content==null)
        {
            return;
        }
        image.sprite = content.CardImage;
        image.rectTransform.sizeDelta = Global.GetImageSize(ImageContainer.rectTransform, image.sprite);
        name.text = content.name.ToString();
        cost.text = content.cost.ToString();
        info0.text = "Attack " + content.Attack + "\n" +
            "Range " + content.Range + "\n" +
            "AOE " + content.AreaAttackRange + "\n";
        info1.text = "Health " + content.MaxHealth + "\n" +
            "Movement " + content.TotalMovement + "\n";        
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    public void OnPointerClick(PointerEventData pData)
    {
        Global.GameInstance.SelectCard(content);
        menu.Close();
    }
}
