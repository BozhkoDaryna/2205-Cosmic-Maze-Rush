using System.Collections.Generic;
using Application.IAP;
using Core;
using UnityEngine;

namespace Application.Services.IAP
{
    [CreateAssetMenu(fileName = "IAPConfig", menuName = "Config/IAPConfig")]
    public class IAPConfig : BaseSettings
    {
        [SerializeField] private List<ProductData> _products;

        public List<ProductData> Products => _products;
    }
}