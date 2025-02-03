import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { RootState } from '@/store';

interface SignalRState {
  status: 'disconnected' | 'connecting' | 'connected';
}

const initialState: SignalRState = {
  status: 'disconnected'
};

export const signalRSlice = createSlice({
  name: 'signalR',
  initialState,
  reducers: {
    setSignalRStatus: (state, action: PayloadAction<'disconnected' | 'connecting' | 'connected'>) => {
      state.status = action.payload;
    }
  }
});

export const { setSignalRStatus } = signalRSlice.actions;
export const selectSignalRStatus = (state: RootState) => state.signalR.status;
export default signalRSlice.reducer;
