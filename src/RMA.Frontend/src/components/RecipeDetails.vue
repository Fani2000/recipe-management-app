<template>
  <v-container v-if="recipe" fluid class="py-6" style="background-color: #fffaf4;position: relative;">

    <!--
    <v-btn
      icon
      color="deep-orange"
      class="ma-2"
      style="position: fixed; top: 16px; left: 16px; z-index: 1000;"
      @click="router.push('/')"
    >
      <v-icon>mdi-arrow-left</v-icon>
    </v-btn>
    -->

    <!--
    <v-img :src="recipe.image || 'https://source.unsplash.com/featured/?pasta,' + recipe.title" height="300px" cover
      class="rounded-lg mb-6" />
      -->

    <div class="image-wrapper mb-6 rounded-lg overflow-hidden" style="position: relative; height: 300px;">
      <v-img
        :src="recipe.image || `https://source.unsplash.com/featured/?pasta,${recipe.title}`"
        height="300"
        cover
        class="rounded-lg"
      />

      <!-- Gradient Overlay -->
      <div class="gradient-overlay"></div>
    </div>


    <!-- Floating Buttons Container -->
    <div class="d-flex justify-end" style="position: absolute; top: 30px; right: 24px; z-index: 10;">
      <!-- Edit Button -->
      <v-btn
        icon
        variant="elevated"
        color="white"
        class="mr-2 button-overlay"
        @click="router.push(`/edit/${recipe.id}`)"
      >
        <v-icon color="deep-orange-darken-3">mdi-pencil</v-icon>
      </v-btn>

      <!-- Delete Button -->
      <v-btn
        icon
        variant="elevated"
        color="white"
        class="button-overlay"
        @click="deleteRecipe"
      >
        <v-icon color="error">mdi-delete</v-icon>
      </v-btn>
    </div>


    <v-card elevation="0" rounded="xl" class="pa-6" style="background-color: #fffdf7;">
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
        <v-expansion-panels v-model="tab" multiple variant="accordion" class="mb-4">
          <!-- Ingredients Panel -->
          <v-expansion-panel>
            <v-expansion-panel-title>
              <v-icon start class="mr-2" color="deep-orange">mdi-silverware-fork-knife</v-icon>
              <span class="font-weight-medium">Ingredients</span>
            </v-expansion-panel-title>

            <v-expansion-panel-text>
              <v-row dense>
                <v-col
                  v-for="ingredient in recipe.ingredients?.$values || []"
                  :key="ingredient.id"
                  cols="12"
                  sm="4"
                >
                  <v-chip
                    class="ma-1"
                    variant="outlined"
                    color="deep-orange-lighten-2"
                    prepend-icon="mdi-food-apple"
                  >
                    {{ ingredient.content }}
                  </v-chip>
                </v-col>
              </v-row>
            </v-expansion-panel-text>
          </v-expansion-panel>

          <!-- Steps Panel -->
          <v-expansion-panel>
            <v-expansion-panel-title>
              <v-icon start class="mr-2" color="indigo">mdi-format-list-numbered</v-icon>
              <span class="font-weight-medium">Steps</span>
            </v-expansion-panel-title>

            <v-expansion-panel-text>
              <v-row dense>
                <v-col
                  v-for="(step, index) in recipe.steps?.$values || []"
                  :key="step.id"
                  cols="12"
                  sm="6"
                >
                  <v-sheet
                    class="pa-3 mb-2"
                    elevation="1"
                    color="deep-orange-lighten-5"
                    rounded
                  >
                    <div class="d-flex align-center mb-1">
                      <v-icon class="mr-2" color="indigo">mdi-arrow-right-bold-circle</v-icon>
                      <strong>Step {{ index + 1 }}</strong>
                    </div>
                    <div>{{ step.content }}</div>
                  </v-sheet>
                </v-col>
              </v-row>
            </v-expansion-panel-text>
          </v-expansion-panel>
        </v-expansion-panels>

      <h4 class="text-h6 font-weight-bold mb-2 mt-6">🏷️ Tags</h4>
        <v-chip v-for="tag in recipe.tags?.$values || []" :key="tag.id" class="ma-1" color="pink lighten-4"
          text-color="pink darken-3" pill small>
          #{{ tag.name }}
        </v-chip>
      </v-card-text>
      <!--
      <v-card-actions class="d-flex justify-end mt-4">
        <v-btn color="deep-orange dark-3" variant="tonal" class="mr-2" text @click="router.push(`/edit/${recipe.id}`)" rounded dark-3>
          <v-icon left>mdi-pencil</v-icon>
        </v-btn>
        <v-btn text @click="deleteRecipe" variant="tonal" color="error" rounded>
          <v-icon left>mdi-delete</v-icon>
        </v-btn>
      </v-card-actions>
      -->
    </v-card>
  </v-container>
</template>

<script lang="ts" setup>
import { useRoute, useRouter } from 'vue-router'
import { onMounted, ref, toRefs, watch } from 'vue'
import { useRecipeStore } from '@/stores/recipeStore'

const route = useRoute()
const store = useRecipeStore()
const recipe = ref<any>(null)
const router = useRouter()

const props = defineProps<{ selectedRecipe: any }>()
const {selectedRecipe} = toRefs(props)

const tab = ref([0,1])

watch(selectedRecipe, (val) => {
  recipe.value = val
})

/*
onMounted(async () => {
  recipe.value = await store.getRecipeById(Number(route.params.id))
})
 */

const deleteRecipe = async () => {
  if (confirm('Are you sure you want to delete this recipe?')) {
    await store.removeRecipe(recipe.value.id)
    router.push('/')
  }
}
</script>

<style scoped>
.gradient-overlay {
  position: absolute;
  top: 0;
  right: 0;
  bottom: 0;
  left: 0;
  border-radius: 0.5rem; /* same as rounded-lg */
  background: linear-gradient(to left, rgba(0, 0, 0, 0.6) 10%, transparent 80%);
  z-index: 2;
}
</style>
