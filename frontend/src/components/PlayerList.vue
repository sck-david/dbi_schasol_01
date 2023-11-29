<template>
    <div>
       <h1>{{ selectedClub.clubName }}</h1>
       <table>
         <thead>
           <tr>
             <th>Firstname</th>
             <th>Lastname</th>
             <th>Date of Birth</th>
             <th>Market Value</th>
             <th>Gender</th>
           </tr>
         </thead>
         <tbody>
           <tr v-for="player in selectedClub.players" :key="player.Id">
             <td>{{ player.firstname }}</td>
             <td>{{ player.lastname }}</td>
             <td>{{ player.gebDat }}</td>
             <td>{{ player.marketvalue }}</td>
             <td>{{ player.gender }}</td>
           </tr>
         </tbody>
       </table>
    </div>
   </template>
   
   <script>
   export default {
    props: {
       selectedClub: Object
    }
   }
   </script>