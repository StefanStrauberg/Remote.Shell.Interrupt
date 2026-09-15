type DeviceBrand = Readonly<{
  alt: string;
  imageSrc: string;
}>;

const fallbackBrand: DeviceBrand = {
  alt: "Network device logo",
  imageSrc: "",
};

const brands: Readonly<Record<string, DeviceBrand>> = Object.freeze({
  Huawei: { alt: "Huawei logo", imageSrc: "/images/Huawei_Logo.png" },
  Juniper: { alt: "Juniper logo", imageSrc: "/images/Juniper_Logo.png" },
  Extreme: { alt: "Extreme logo", imageSrc: "/images/Extreme_Logo.png" },
  Cisco: { alt: "Cisco logo", imageSrc: "/images/Cisco_Logo.png" },
  FortiGate: { alt: "Fortinet logo", imageSrc: "/images/Fortinet_Logo.png" },
});

export function getNetworkDeviceBrand(type: string): DeviceBrand {
  return brands[type] ?? fallbackBrand;
}
