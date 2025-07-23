<template>
  <AddRecipe :editMode="true" :initialRecipe="recipe" />
</template>

<script setup lang="ts">
import { ref, onMounted, watch } from 'vue'
import { useRoute } from 'vue-router'
import { useRecipeStore } from '@/stores/recipeStore'
import AddRecipe from './AddRecipe.vue'

const route = useRoute()
const store = useRecipeStore()
const recipe = ref(null)

watch(recipe, (newVal) => {
  console.log(newVal)
})

onMounted(async () => {
  const id = Number(route.params.id)
  recipe.value = await store.getRecipeById(id)
})
</script>
