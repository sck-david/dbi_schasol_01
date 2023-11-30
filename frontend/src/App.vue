<template>
  <div id="app">
     <h1>Club Selection</h1>
     <select v-model="selectedClubId">
       <option v-for="club in clubs" :key="club.Id" :value="club.Id">
         {{ club.clubName }}
       </option>
     </select>
 
     <PlayerList :selectedClub="selectedClub" />
  </div>
 </template>
 
 <script>
 import PlayerList from './components/PlayerList.vue';
 import axios from 'axios';
 
 export default {
  name: 'App',
  components: {
     PlayerList
  },
  data() {
     return {
       clubs: [],
       selectedClubId: null,
       selectedClub: null
     };
  },
  created() {
     axios.get('http://localhost:5053/api/getC')
       .then(response => {
         this.clubs = response.data;
         this.selectedClubId = this.clubs[0].Id;
         this.selectedClub = this.clubs[0];
       });
  },
  watch: {
     selectedClubId(newValue) {
       this.selectedClub = this.clubs.find(club => club.Id === newValue);
     }
  }
 }
 </script>