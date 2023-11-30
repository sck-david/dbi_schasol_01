<template>
  <div>
     <h2>{{ selectedClub.clubName }} Players</h2>
     <ul>
       <li v-for="player in selectedClub.players" :key="player.id">
         {{ player.firstname }} {{ player.lastname }}
       </li>
     </ul>
  </div>
 </template>
 
 <script>
 export default {
  name: 'PlayerList',
  props: {
     selectedClub: {
       type: Object,
       required: true
     }
  }
 }
 </script>