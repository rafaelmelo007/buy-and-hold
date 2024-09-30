import { Routes } from '@angular/router';
import { LoginPage } from './account/pages/login/login.component';
import { RegisterPage } from './account/pages/register/register.component';
import { ForgotPasswordPage } from './account/pages/forgot-password/forgot-password.component';
import { MoodPage } from './market-mood/pages/mood/mood.component';
import { WalletsPage } from './wallets/pages/wallets.component';
import { SaveWalletPage } from './wallets/pages/save-wallet.component';
import { MarketBreadthPage } from './market-breadth/pages/market-breadth/market-breadth.component';
import { SymbolsPage } from './symbols/pages/symbols.component';
import { SymbolDetailPage } from './symbols/pages/symbol-detail.component';

export const routes: Routes = [
  /* Feature: Account */
  { path: 'pages/account/login', component: LoginPage },
  { path: 'pages/account/register', component: RegisterPage },
  { path: 'pages/account/forgot-password', component: ForgotPasswordPage },

  /* Feature: MarketMood */
  { path: 'pages/market-mood/mood', component: MoodPage },

  /* Feature: MarketBreadth */
  { path: 'pages/market-breadth', component: MarketBreadthPage },

  /* Feature: Wallets */
  { path: 'pages/wallets', component: WalletsPage },
  { path: 'pages/wallets/save-wallet/:id', component: SaveWalletPage },

  /* Feature: Symbols */
  { path: 'pages/symbols', component: SymbolsPage },
  { path: 'pages/symbols/:symbol', component: SymbolDetailPage },

  { path: '**', redirectTo: '/pages/symbols' },
];
