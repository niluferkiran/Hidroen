import * as maptilersdk from '@maptiler/sdk';

// API anahtarınızı ayarlayın
maptilersdk.config.apiKey = 'fpTyZ0mESyZNhLxa83zq';  // Kendi API anahtarınızı buraya ekleyin

// Haritayı oluşturun
const map = new maptilersdk.Map({
    container: 'map', // Haritanın render edileceği div'in id'si
    style: "https://api.maptiler.com/maps/basic-v2/style.json", // Kullanmak istediğiniz harita stili
    center: [16.62662018, 49.2125578], // Başlangıç konumu [lng, lat]
    zoom: 14, // Başlangıç zoom seviyesi
});
