<template>
  <div>
    <h2>{{ selectedClub.clubName }} Players</h2>
    <ul>
      <li v-for="player in selectedClub.players" :key="player.id">
        {{ player.firstname }} {{ player.lastname }}
        <button @click="editPlayer(player.id)">Edit</button>
        <button @click="deletePlayer(player.id)">Delete</button>
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
      required: true,
    },
  },
  methods: {
    editPlayer(player) {
      console.log('Edit player with id:', player);
      this.$emit('playerEdit', player);
    },
    deletePlayer(playerId) {
      fetch(`http://localhost:5053/deletePlayer/${playerId}`, {
        method: 'DELETE',
        headers: {
          'Content-Type': 'application/json',
        },
      })
        .then(response => {
          if (!response.ok) {
            throw new Error(`HTTP error! Status: ${response.status}`);
          }
          console.log('Player deleted successfully');
          // Emit an event to notify the parent about the deletion
          this.$emit('playerDeleted', playerId);
        })
        .catch(error => {
          console.error('Error deleting player:', error.message);
        });
    },
  },
};
</script>
