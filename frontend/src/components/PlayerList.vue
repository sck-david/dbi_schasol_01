

<template>
  <div>
    <h2>{{ selectedClub.clubName }} Players</h2>
    <ul>
      <li
        v-for="player in selectedClub.players"
        :key="player.id"
        class="player-card"
        draggable="true"
        @dragstart="startDrag(player)"
        @dragover.prevent
        @drop="endDrag(player)"
      >
      
          <span>
            <input v-model="player.firstname" :disabled="player !== editingPlayer" />
            <input v-model="player.lastname" :disabled="player !== editingPlayer" />
            <input v-model="formattedGebDat" type="date" :disabled="player !== editingPlayer" />
            <select v-model="player.gender" :disabled="player !== editingPlayer">
              <option value="0">Male</option>
              <option value="0">Female</option>
              <!-- Add other gender options if needed -->
            </select>
            <button @click="editPlayer(player)">Edit</button>
            <button @click="savePlayerChanges">Save Changes</button>
            <button @click="deletePlayer(player.id)">Delete</button>
          </span>
        
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
  data() {
    return {
      editingPlayer: null,
    };
  },
  computed: {
    formattedGebDat: {
      get() {
        // Convert the player's gebDat to ISO 8601 format
        return this.editingPlayer ? new Date(this.editingPlayer.gebDat).toISOString().split('T')[0] : '';
      },
      set(value) {
        // When the user changes the input, update the editingPlayer's gebDat
        this.editingPlayer.gebDat = value;
      },
    },
  },
  methods: {
    startDrag(player) {
      // Set the data to be transferred during drag
      event.dataTransfer.setData('text/plain', player.id);
      // Save the current index of the dragged player
      this.draggedPlayerIndex = this.selectedClub.players.indexOf(player);
    },

    endDrag() {
      // Get the ID of the dragged player
      const playerId = event.dataTransfer.getData('text/plain');
      // Find the dragged player in the list
      const draggedPlayer = this.selectedClub.players.find((p) => p.id === playerId);

      if (draggedPlayer) {
        // Remove the player from the original position
        // this.selectedClub.players.splice(this.draggedPlayerIndex, 1);
        // Find the index where the player was dropped
        //const dropIndex = this.selectedClub.players.indexOf(player);
        // Insert the player at the new position
        // this.selectedClub.players.splice(dropIndex, 0, draggedPlayer);
      }
    },
    editPlayer(player) {
      this.editingPlayer = player;
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
    savePlayerChanges() {
      if (this.editingPlayer) {
        const playerId = this.editingPlayer.id;
        fetch(`http://localhost:5053/updatePlayer/${playerId}`, {
          method: 'PUT',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({
            id: playerId,
            firstname: this.editingPlayer.firstname,
            lastname: this.editingPlayer.lastname,
            gebDat: this.editingPlayer.gebDat,
            gender: this.editingPlayer.gender,
            // Add other fields as needed
          }),
        })
          .then(response => {
            if (!response.ok) {
              throw new Error(`HTTP error! Status: ${response.status}`);
            }
            console.log('Player updated successfully');
            // Optionally, you can emit an event to inform the parent about the update
            this.$emit('playerUpdated', this.editingPlayer);
            // Clear the editingPlayer after successful update
            this.editingPlayer = null;
          })
          .catch(error => {
            console.error('Error updating player:', error.message);
          });
      }
    },
    formatDate(dateString) {
      // Format the date string to "yyyy-MM-dd" format
      const date = new Date(dateString);
      const year = date.getFullYear();
      const month = `${date.getMonth() + 1}`.padStart(2, '0');
      const day = `${date.getDate()}`.padStart(2, '0');
      return `${year}-${month}-${day}`;
    },
  },
};
</script>

<style scoped>
  /* Background styling */
  body {
    /* background: url('your-soccer-background-image-url.jpg') center center fixed;*/
    background-size: cover;
    margin: 0;
    padding: 0;
    font-family: 'Arial', sans-serif;
    color: #fff; /* Text color on top of the background */
  }

  /* General styling for the player list container */
  .player-list {
    list-style: none;
    padding: 0;
  }

  /* Styling for each player card */
  .player-card {
    cursor: move;
    border: 2px solid #fff;
    padding: 15px;
    margin-bottom: 15px;
    background-color: rgba(0, 0, 0, 0.7); /* Semi-transparent black background */
  }

  /* Styling for the player card input fields */
  .player-card input {
    font-size: medium;
    margin-right: 10px;
    margin-bottom: 5px;
    padding: 5px;
    background-color: rgba(255, 255, 255, 0.7); /* Semi-transparent white background */
    color: #000; /* Text color on top of the input background */
  }

  /* Styling for the player card select field */
  .player-card select {
    margin-right: 10px;
    margin-bottom: 5px;
    padding: 5px;
    background-color: rgba(255, 255, 255, 0.7); /* Semi-transparent white background */
    color: #000; /* Text color on top of the select background */
  }

  /* Styling for the buttons within the player card */
  .player-card button {
    margin-bottom: 5px;
    padding: 8px 15px;
    cursor: pointer;
    background-color: #fff;
    color: #000; /* Text color on top of the button background */
    border: none;
    border-radius: 5px;
    transition: background-color 0.3s ease;
  }

  .player-card button:hover {
    background-color: #555; /* Darker background on hover */
    color: #fff; /* Lighter text on hover */
  }

  /* Styling for the editing state of input fields */
  .player-card input:disabled {
    background-color: #ddd; /* Lighter background when disabled */
  }
</style>