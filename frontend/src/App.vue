<template style="background-color: #006400">
  <div id="app" style="font-family: 'Comic Sans MS'; background-color: #00FF00;">
    
     <h1 style="color: #FF00FF;">Club Selection</h1>
     <img src="https://i.pinimg.com/originals/ca/6c/74/ca6c744333366d89b3824449cb844c2e.gif" alt="Your GIF" style="position: absolute; top: 10px; right: 10px; width: 200px; height: 200px;">
     <select v-model="selectedClubId" style="color: #FF00FF; background-color: #00FF00;">
       <option v-for="club in clubs" :key="club.id" :value="club.id">
         {{ club.clubName }}
       </option>
     </select>
 
     <PlayerList :selectedClub="selectedClub" @playerDeleted="handlePlayerDeleted" />
  </div>
  <img src="https://media.tenor.com/TmvqZuZdfqQAAAAM/holy-moly-holy.gif" alt="Your GIF" style="width: 200px; height: 200px;">
  <audio controls autoplay loop style="width: 80%; max-width: 400px;">
      <source src="..\bgmusic.mp3" type="audio/mp3">
      Your browser does not support the audio element.
    </audio>
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
      selectedClub: { Id: 0, clubName: '', players: [] },
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
  },
  methods: {
    handlePlayerDeleted(playerId) {
      this.selectedClub.players = this.selectedClub.players.filter(player => player.id !== playerId);
    }
  },
}
</script>
