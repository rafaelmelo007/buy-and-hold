export interface Symbol {
  id: number;
  name: string;
  lastPrice: number;
  lastPriceDate: Date;
  percentToHigh: number;
  percentToLow: number;
  buyMood: number;
  sellMood: number;
}
