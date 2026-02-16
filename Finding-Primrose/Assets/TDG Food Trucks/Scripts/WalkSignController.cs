using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environment {

    public class WalkSignController : MonoBehaviour {

        [SerializeField] Material[] signMaterials;
        [SerializeField] float changeTimer = 10f;
        [SerializeField] bool walkIndicator = false;
        [SerializeField] Material[] materials;

        private void Start() {
            materials = GetComponent<MeshRenderer>().materials;
            //SetMaterial(signMaterials[1]);
        }

        private void Update() {
            if (changeTimer > 0) {
                changeTimer -= Time.deltaTime;
            } else if (changeTimer <= 0) {
                changeTimer = 10f;
                if (!walkIndicator) {
                    SetMaterial(signMaterials[0]);
                } else {
                    SetMaterial(signMaterials[1]);
                }

                walkIndicator = !walkIndicator;
            }
        }

        void SetMaterial(Material newMaterial) {
            materials[1] = newMaterial;
            GetComponent<MeshRenderer>().materials = materials;
        }

    }
}
