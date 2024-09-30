export interface Symbol {
  id?: number;
  name?: string;
  aliases?: string;
  lastPrice?: number;
  lastPriceDate?: Date;
  last30DaysPrice?: number;
  last100DaysPrice?: number;
  last6MonthsPrice?: number;
  last30DaysVariation?: number;
  last100DaysVariation?: number;
  last6MonthsVariation?: number;
  recommendedStrategyId?: number;
}
