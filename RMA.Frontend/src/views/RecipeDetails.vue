<template>
  <v-container v-if="recipe">
    <v-img :src="recipe.imageUrl" height="300px" class="mb-4" />
    <v-card>
      <v-card-title>{{ recipe.title }}</v-card-title>
      <v-card-subtitle>{{ recipe.description }}</v-card-subtitle>
      <v-card-text>
        <h4>Tags</h4>
        <v-chip v-for="tag in recipe.tags" :key="tag" class="ma-1">{{ tag }}</v-chip>
        <h4 class="mt-4">Instructions</h4>
        <p>{{ recipe.instructions }}</p>
      </v-card-text>
    </v-card>
  </v-container>
</template>

<script lang="ts" setup>
import { useRoute } from 'vue-router'
import { onMounted, ref } from 'vue'
import { useRecipeStore } from '@/stores/recipeStore'

const route = useRoute()
const store = useRecipeStore()
const recipe = ref<any>(null)

onMounted(async () => {
  recipe.value = await store.getRecipeById(Number(route.params.id))
  console.log(recipe.value)
})
</script>

