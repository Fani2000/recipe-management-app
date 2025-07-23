<template>
  <v-container class="py-6" fluid style="background-color: #fffaf4;height: 100vh;display: flex; align-items: center;" >
    <v-card class="pa-6 mx-auto" max-width="800" elevation="6" rounded="xl" style="background-color: #fffbe6;">
      <v-card-title class="text-h4 font-weight-bold mb-4">
        {{ editMode ? '✏️ Edit Recipe' : '🍳 Add New Recipe' }}
      </v-card-title>

      <v-tabs v-model="tab" background-color="#fffaf4" class="mb-4">
        <v-tab>Information</v-tab>
        <v-tab>Ingredients</v-tab>
        <v-tab>Steps</v-tab>
        <v-tab>Tags</v-tab>
        <v-tab>Review</v-tab>
      </v-tabs>

      <v-window v-model="tab">
        <v-window-item :value="0">
          <v-card-text>
            <v-text-field v-model="title" label="Title" outlined dense required class="mb-3" />
            <v-textarea v-model="description" label="Description" outlined dense rows="3" required class="mb-3" />
            <v-text-field v-model="image" label="Image URL" outlined dense class="mb-3" />
            <v-text-field v-model="cookingTime" label="Cooking Time (minutes)" type="number" outlined dense required class="mb-3" />
          </v-card-text>
        </v-window-item>

        <v-window-item :value="1">
          <v-card-text>
            <h4 class="text-subtitle-1 font-weight-medium mb-2">🧂 Ingredients</h4>
            <div v-for="(item, index) in ingredients" :key="index" class="d-flex mb-2">
              <v-text-field v-model="ingredients[index]" label="Ingredient" class="flex-grow-1 mr-2" dense outlined />
              <v-btn icon @click="removeIngredient(index)"><v-icon color="red">mdi-delete</v-icon></v-btn>
            </div>
            <v-btn text small color="deep-orange" @click="addIngredient" class="mb-4">+ Add Ingredient</v-btn>
          </v-card-text>
        </v-window-item>

        <v-window-item :value="2">
          <v-card-text>
            <h4 class="text-subtitle-1 font-weight-medium mb-2">👨‍🍳 Steps</h4>
            <div v-for="(item, index) in steps" :key="index" class="d-flex mb-2">
              <v-textarea v-model="steps[index]" label="Step" class="flex-grow-1 mr-2" dense outlined auto-grow rows="2" />
              <v-btn icon @click="removeStep(index)"><v-icon color="red">mdi-delete</v-icon></v-btn>
            </div>
            <v-btn text small color="deep-orange" @click="addStep" class="mb-4">+ Add Step</v-btn>
          </v-card-text>
        </v-window-item>

        <v-window-item :value="3">
          <v-card-text>
            <v-text-field v-model="tags" label="Tags (comma separated)" outlined dense />
          </v-card-text>
        </v-window-item>

        <v-window-item :value="4">
           <v-card-text>
            <h4 class="text-subtitle-1 font-weight-medium mb-4">📋 Review</h4>
            <v-list dense style="background-color: #fffaf4;">
              <v-list-item><v-list-item-title><strong>Title:</strong> {{ title }}</v-list-item-title></v-list-item>
              <v-list-item><v-list-item-title><strong>Description:</strong> {{ description }}</v-list-item-title></v-list-item>
              <v-list-item><v-list-item-title><strong>Image URL:</strong> {{ image }}</v-list-item-title></v-list-item>
              <v-list-item><v-list-item-title><strong>Cooking Time:</strong> {{ cookingTime }} minutes</v-list-item-title></v-list-item>
              <v-list-item>
                <v-list-item-title>
                  <strong>Ingredients:</strong>
                  <v-chip-group column>
                    <v-chip v-for="(i, idx) in ingredients" :key="idx" class="ma-1" small>{{ i }}</v-chip>
                  </v-chip-group>
                </v-list-item-title>
              </v-list-item>
              <v-list-item>
                <v-list-item-title>
                  <strong>Steps:</strong>
                  <ol class="pl-4">
                    <li v-for="(s, idx) in steps" :key="idx">{{ s }}</li>
                  </ol>
                </v-list-item-title>
              </v-list-item>
              <v-list-item><v-list-item-title><strong>Tags:</strong> {{ tags }}</v-list-item-title></v-list-item>
            </v-list>
          </v-card-text>
        </v-window-item>
      </v-window>

      <v-card-actions class="justify-end">
        <v-btn color="deep-orange" class="mr-2" @click="submitRecipe">{{ editMode ? 'Update' : 'Submit' }}</v-btn>
        <v-btn text to="/">Cancel</v-btn>
      </v-card-actions>
    </v-card>
  </v-container>
</template>

<script setup lang="ts">
import { ref, watch, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useRecipeStore } from '@/stores/recipeStore'

interface Props {
  editMode?: boolean
  initialRecipe?: any
}

const props = defineProps<Props>()
const router = useRouter()
const store = useRecipeStore()

const tab = ref(0)
const title = ref('')
const description = ref('')
const image = ref('')
const cookingTime = ref(0)
const ingredients = ref<string[]>([''])
const steps = ref<string[]>([''])
const tags = ref('')

onMounted(() => {
  if (props.initialRecipe) {
    const recipe = props.initialRecipe
    title.value = recipe.title || ''
    description.value = recipe.description || ''
    image.value = recipe.image || ''
    cookingTime.value = recipe.cookingTimeMinutes || 0
    ingredients.value = recipe.ingredients?.$values?.map((i: any) => i.content) || ['']
    steps.value = recipe.steps?.$values?.map((s: any) => s.content) || ['']
    tags.value = recipe.tags?.$values?.map((t: any) => t.name).join(', ') || ''
  }
})

watch(() => props.initialRecipe, (newRecipe) => {
  if (newRecipe) {
    title.value = newRecipe.title || ''
    description.value = newRecipe.description || ''
    image.value = newRecipe.image || ''
    cookingTime.value = newRecipe.cookingTimeMinutes || 0
    ingredients.value = newRecipe.ingredients?.$values?.map((i: any) => i.content) || ['']
    steps.value = newRecipe.steps?.$values?.map((s: any) => s.content) || ['']
    tags.value = newRecipe.tags?.$values?.map((t: any) => t.name).join(', ') || ''
  }
}, { immediate: true })

const addIngredient = () => ingredients.value.push('')
const removeIngredient = (index: number) => ingredients.value.splice(index, 1)
const addStep = () => steps.value.push('')
const removeStep = (index: number) => steps.value.splice(index, 1)

const submitRecipe = async () => {
  const payload = {
    title: title.value,
    description: description.value,
    image: image.value,
    cookingTimeMinutes: cookingTime.value,
    ingredients: ingredients.value.map(content => ({ content })),
    steps: steps.value.map(content => ({ content })),
    tags: tags.value.split(',').map(name => ({ name: name.trim() }))
  }

  if (props.editMode && props.initialRecipe) {
    await store.modifyRecipe(props.initialRecipe.id, payload)
  } else {
    await store.addRecipe(payload)
  }

  router.push('/')
}
</script>
