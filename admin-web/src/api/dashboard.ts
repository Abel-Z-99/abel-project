import http from './http'
import type { DashboardSummary } from '../types/models'

export function getDashboardSummary() {
  return http.get<unknown, DashboardSummary>('/admin/dashboard/summary')
}
