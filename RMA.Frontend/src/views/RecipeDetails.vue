<template>
  <v-container v-if="recipe" fluid class="py-6" style="background-color: #fffaf4;">

    <v-btn
      icon
      color="deep-orange"
      class="ma-2"
      style="position: fixed; top: 16px; left: 16px; z-index: 1000;"
      @click="router.push('/')"
    >
      <v-icon>mdi-arrow-left</v-icon>
    </v-btn>

    <v-img :src="recipe.image || 'https://source.unsplash.com/featured/?pasta,' + recipe.title" height="300px" cover
      class="rounded-lg mb-6" />

    <v-card elevation="6" rounded="xl" class="pa-6" style="background-color: #fffdf7;">
      <v-card-title class="text-h4 font-weight-bold mb-1">
        🍽️ {{ recipe.title }}
      </v-card-title>
      <v-card-subtitle class="mb-2 text-grey-darken-1">
        {{ recipe.description }}
      </v-card-subtitle>

      <div class="d-flex align-center mb-4">
        <v-icon color="orange darken-2" class="mr-2">mdi-timer</v-icon>
        <span class="text-subtitle-2 font-weight-medium orange--text">
          {{ recipe.cookingTimeMinutes }} minutes cooking time
        </span>
      </div>

      <v-divider class="my-4" />

      <v-card-text>
        <v-expansion-panels v-model="tab" variant="accordion" class="mb-4">
          <v-expansion-panel>
            <v-expansion-panel-title>
              🧂 Ingredients
            </v-expansion-panel-title>
            <v-expansion-panel-text>
              <v-list dense>
                <v-list-item v-for="ingredient in recipe.ingredients?.$values || []" :key="ingredient.id">
                  <v-list-item-content>
                    <v-icon start icon="mdi-circle-small" color="deep-orange" />
                    {{ ingredient.content }}
                  </v-list-item-content>
                </v-list-item>
              </v-list>
            </v-expansion-panel-text>
          </v-expansion-panel>

          <v-expansion-panel>
            <v-expansion-panel-title>
              👨‍🍳 Steps
            </v-expansion-panel-title>
            <v-expansion-panel-text>
              <v-list dense>
                <v-list-item v-for="(step, index) in recipe.steps?.$values || []" :key="step.id">
                  <v-list-item-content>
                    <strong class="mr-2">Step {{ index + 1 }}:</strong>
                    {{ step.content }}
                  </v-list-item-content>
                </v-list-item>
              </v-list>
            </v-expansion-panel-text>
          </v-expansion-panel>
        </v-expansion-panels>

        <h4 class="text-h6 font-weight-bold mb-2 mt-6">🏷️ Tags</h4>
        <v-chip v-for="tag in recipe.tags?.$values || []" :key="tag.id" class="ma-1" color="pink lighten-4"
          text-color="pink darken-3" pill small>
          #{{ tag.name }}
        </v-chip>
      </v-card-text>
      <v-card-actions class="d-flex justify-end mt-4">
        <v-btn color="deep-orange" class="mr-2" text @click="router.push(`/edit/${recipe.id}`)">
          <v-icon left>mdi-pencil</v-icon> Edit
        </v-btn>
        <v-btn text @click="deleteRecipe">
          <v-icon left>mdi-delete</v-icon> Delete
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-container>
</template>

<script lang="ts" setup>
import { useRoute, useRouter } from 'vue-router'
import { onMounted, ref } from 'vue'
import { useRecipeStore } from '@/stores/recipeStore'

const route = useRoute()
const store = useRecipeStore()
const recipe = ref<any>(null)
const router = useRouter()

const tab = ref(0)

onMounted(async () => {
  recipe.value = await store.getRecipeById(Number(route.params.id))
})

const deleteRecipe = async () => {
  if (confirm('Are you sure you want to delete this recipe?')) {
    await store.removeRecipe(recipe.value.id)
    router.push('/')
  }
}
</script>
