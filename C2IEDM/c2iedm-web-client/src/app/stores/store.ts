import { createContext, useContext } from "react";
import PersonStore from "./personStore";
import AbsolutePointStore from "./absolutePointStore";
import CommonStore from "./commonStore";
import ModalStore from "./modalStore";
import UserStore from "./userStore";

interface Store {
  personStore: PersonStore;
  absolutePointStore: AbsolutePointStore;
  commonStore: CommonStore;
  userStore: UserStore;
  modalStore: ModalStore;
}

export const store: Store = {
  personStore: new PersonStore(),
  absolutePointStore: new AbsolutePointStore(),
  commonStore: new CommonStore(),
  userStore: new UserStore(),
  modalStore: new ModalStore(),
};

export const StoreContext = createContext(store);

export function useStore() {
  return useContext(StoreContext);
}
