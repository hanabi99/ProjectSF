using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cute2dDemo
{
    public class CharacterChangerFantasyKit : MonoBehaviour
    {
        private GameObject PlayerCharacter;
        public GameObject Knight;
        public GameObject Berserker;
        public GameObject Paladin;
        public GameObject Mage;
        public GameObject Priest;

        // Start is called before the first frame update
        void Start()
        {
            PlayerCharacter = Instantiate(Knight);
        }

        // Update is called once per frame
        void Update()
        {

        }
        public void knight()
        {
            Destroy(PlayerCharacter);
            PlayerCharacter = Instantiate(Knight);
        }
        public void berserker()
        {
            Destroy(PlayerCharacter);
            PlayerCharacter = Instantiate(Berserker);
        }
        public void paladin()
        {
            Destroy(PlayerCharacter);
            PlayerCharacter = Instantiate(Paladin);
        }
        public void mage()
        {
            Destroy(PlayerCharacter);
            PlayerCharacter = Instantiate(Mage);
        }
        public void priest()
        {
            Destroy(PlayerCharacter);
            PlayerCharacter = Instantiate(Priest);
        }
    }

}
