<template>
  <div id="app">
     <h1>Club Selection</h1>
     <select v-model="selectedClubId">
       <option v-for="club in clubs" :key="club.id" :value="club.id">
         {{ club.clubName }}
       </option>
     </select>
 
     <PlayerList :selectedClub="selectedClub" />
  </div>
 </template>
 
 <script>
 import PlayerList from './components/PlayerList.vue';
 
 export default {
  name: 'App',
  components: {
     PlayerList
  },
  data() {
     return {
       clubs: [],
       selectedClubId: 0,
       selectedClub: { Id: 0, clubName: '' }
     };
  },
  async created() {
 try {
    const response = await fetch('http://localhost:5053/getC');
    const data = await response.json();
    this.clubs = data;
    console.log(this.clubs);
    if (this.clubs.length > 0) {
      this.selectedClubId = this.clubs[0].id;
      console.log(this.selectedClubId);
      this.selectedClub = this.clubs[0];
    } else {
      console.error('No clubs were found');
      // You can handle the error appropriately here (e.g., set a default value)
      this.selectedClub = { Id: 0, clubName: 'No clubs available' };    
    }
 } catch (error) {
    console.error('Error:', error);
 }
},
  watch: {
     selectedClubId(newValue) {
       this.selectedClub = this.clubs.find(club => club.id === newValue);
     }
  }
 }
 </script>