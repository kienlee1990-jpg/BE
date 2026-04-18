<template>
  <div :id="`tools-${tableId}`" class="table-tools">
    <button type="button" class="table-tools-trigger" @click="toggleMenu">
      <i class="bi bi-layout-three-columns"></i>
      <span>Tùy chọn cột</span>
    </button>

    <div v-if="open" class="table-tools-menu">
      <div class="table-tools-head">
        <strong>Cột hiển thị</strong>
        <div class="table-tools-actions">
          <button type="button" @click="showAll">Tất cả</button>
          <button type="button" @click="hideAll">Bỏ hết</button>
        </div>
      </div>

      <label v-for="column in columns" :key="column.index" class="table-tools-option">
        <input v-model="visibleColumns" type="checkbox" :value="column.index" />
        <span>{{ column.label }}</span>
      </label>
    </div>
  </div>
</template>

<script setup>
import { computed, nextTick, onBeforeUnmount, onMounted, ref, watch } from 'vue'

const props = defineProps({
  tableId: {
    type: String,
    required: true
  }
})

const open = ref(false)
const columns = ref([])
const visibleColumns = ref([])

const hiddenSet = computed(() => new Set(columns.value.map(column => column.index).filter(index => !visibleColumns.value.includes(index))))

const getTable = () => document.getElementById(props.tableId)

const collectColumns = () => {
  const table = getTable()
  if (!table) return

  const headerCells = Array.from(table.querySelectorAll('thead th'))
  columns.value = headerCells.map((headerCell, index) => ({
    index,
    label: headerCell.textContent?.trim() || `Cột ${index + 1}`
  }))

  if (!visibleColumns.value.length || visibleColumns.value.length > columns.value.length) {
    visibleColumns.value = columns.value.map(column => column.index)
  } else {
    const existing = new Set(columns.value.map(column => column.index))
    visibleColumns.value = visibleColumns.value.filter(index => existing.has(index))
  }

  applyVisibility()
}

const applyVisibility = () => {
  const table = getTable()
  if (!table) return

  const rows = Array.from(table.querySelectorAll('tr'))
  rows.forEach(row => {
    const cells = Array.from(row.children)
    cells.forEach((cell, index) => {
      cell.style.display = hiddenSet.value.has(index) ? 'none' : ''
    })
  })
}

const toggleMenu = async () => {
  open.value = !open.value
  if (open.value) {
    await nextTick()
    collectColumns()
  }
}

const showAll = () => {
  visibleColumns.value = columns.value.map(column => column.index)
}

const hideAll = () => {
  visibleColumns.value = []
}

const handleDocumentClick = (event) => {
  const target = event.target
  if (!(target instanceof Node)) return
  const host = document.getElementById(`tools-${props.tableId}`)
  if (host && !host.contains(target)) {
    open.value = false
  }
}

watch(visibleColumns, () => {
  applyVisibility()
}, { deep: true })

onMounted(async () => {
  await nextTick()
  collectColumns()
  document.addEventListener('click', handleDocumentClick)
})

onBeforeUnmount(() => {
  document.removeEventListener('click', handleDocumentClick)
})
</script>

<style>
.table-tools{position:relative;display:flex;justify-content:flex-end;margin-bottom:12px}
.table-tools-trigger{display:inline-flex;align-items:center;gap:8px;padding:8px 14px;border:1px solid #d7deea;border-radius:999px;background:#fff;color:#0f172a;font-size:13px;font-weight:600;box-shadow:0 8px 20px rgba(15,23,42,.05)}
.table-tools-trigger:hover{background:#f8fafc}
.table-tools-menu{position:absolute;right:0;top:calc(100% + 8px);width:280px;max-height:360px;overflow:auto;padding:14px;background:#fff;border:1px solid #e2e8f0;border-radius:18px;box-shadow:0 18px 40px rgba(15,23,42,.16);z-index:40}
.table-tools-head{display:flex;align-items:center;justify-content:space-between;gap:10px;margin-bottom:10px}
.table-tools-actions{display:flex;gap:8px}
.table-tools-actions button{border:none;background:transparent;color:#2563eb;font-size:12px;font-weight:700}
.table-tools-option{display:flex;align-items:center;gap:10px;padding:8px 4px;font-size:14px;color:#0f172a}
.managed-table{width:100%;border-collapse:separate;border-spacing:0;background:#fff}
.managed-table thead th{position:relative;background:#f8fafc;color:#0f172a;font-size:12px;font-weight:700;text-transform:uppercase;letter-spacing:.04em;padding:14px 16px;border-bottom:1px solid #dbe4ef;white-space:nowrap}
.managed-table tbody td{padding:14px 16px;border-bottom:1px solid #eef2f7;vertical-align:middle}
.managed-table tbody tr:hover{background:#f8fbff}
.managed-table tbody tr:last-child td{border-bottom:none}
</style>
